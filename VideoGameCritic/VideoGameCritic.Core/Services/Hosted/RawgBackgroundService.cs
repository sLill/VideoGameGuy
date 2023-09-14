using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using VideoGameCritic.Configuration;
using VideoGameCritic.Data;

namespace VideoGameCritic.Core
{
    public class RawgBackgroundService : BackgroundService
    {
        #region Fields..
        private readonly ILogger<RawgBackgroundService> _logger;
        private readonly IOptions<RawgApiSettings> _settings;
        private readonly ISystemStatusRepository _systemStatusRepository;
        private readonly IGamesRepository _gamesRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        #endregion Fields..

        #region Properties..
        private string LocalCachePath
            => Path.Combine(AppContext.BaseDirectory, _settings.Value.LocalCache_RelativePath);
        #endregion Properties..

        #region Constructors..
        public RawgBackgroundService(ILogger<RawgBackgroundService> logger,
                                     IOptions<RawgApiSettings> settings,
                                     ISystemStatusRepository systemStatusRepository,
                                     IGamesRepository gamesRepository,
                                     IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _settings = settings;
            _systemStatusRepository = systemStatusRepository;
            _systemStatusRepository = systemStatusRepository;
            _gamesRepository = gamesRepository;
            _httpClientFactory = httpClientFactory;
        }
        #endregion Constructors..

        #region Methods..
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var currentSystemStatus = _systemStatusRepository.GetCurrentStatus();
                double daysSinceUpdate = (DateTime.UtcNow - currentSystemStatus.Rawg_UpdatedOnUtc.Value).TotalDays;

                if (daysSinceUpdate >= _settings.Value.LocalCache_UpdateInterval_Days)
                {
                    await ImportGameDataAsync_DEBUG();
                    //await PollAndCacheAsync();

                    currentSystemStatus.Rawg_UpdatedOnUtc = DateTime.UtcNow;
                    await _systemStatusRepository.UpdateAsync(currentSystemStatus);
                }

                await Task.Delay(TimeSpan.FromHours(2), cancellationToken);
            }
        }

        private async Task PollAndCacheAsync()
        {
            _logger.LogInformation($"[RAWG] Beginning update..");
            var stopwatch = Stopwatch.StartNew();

            await UpdateLocalGameDataAsync().ConfigureAwait(false);

            stopwatch.Stop();
            _logger.LogInformation($"[RAWG] Update complete. Total time: {stopwatch.ElapsedMilliseconds / 1000} seconds");
        }

        private async Task UpdateLocalGameDataAsync()
        {
            try
            {
                _logger.LogInformation($"[RAWG] Updating local game data..");

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);

                    int pageNumber = 1;
                    int retryLimit = _settings.Value.RetryLimit;
                    int retries = 0;

                    while (true)
                    {
                        string baseUrl = Path.Combine(_settings.Value.ApiUrl, _settings.Value.Endpoints["Games"]);
                        string requestUrl = $"{baseUrl}?key={_settings.Value.ApiKey}&page={pageNumber}&page_size={_settings.Value.Response_PageSize}";

                        // Request and cache file if it is old or does not exist
                        var response = await httpClient.GetAsync(requestUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            // Cache response to db
                            string responseString = await response.Content.ReadAsStringAsync();
                            await CacheGameDataAsync(responseString).ConfigureAwait(false);

                            // Check for next page
                            JObject json = JsonConvert.DeserializeObject<JObject>(responseString);

                            var nextToken = json["next"];
                            if (nextToken.Type == JTokenType.Null)
                                break;
                            else
                            {
                                retries = 0;
                                pageNumber++;
                            }
                        }

                        // Retry on timeouts
                        else if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
                        {
                            if (retries >= retryLimit)
                            {
                                _logger.LogError($"[RAWG] Request retry limit exceeded for {requestUrl}. Aborting.");

                                retries = 0;
                                pageNumber++;
                            }
                            else
                            {
                                _logger.LogWarning($"[RAWG] Request timed out {requestUrl}. Retrying ({retries}/{retryLimit})");
                                retries++;
                            }
                        }

                        // End of data
                        else if (response.StatusCode == HttpStatusCode.BadGateway)
                            break;

                        else
                            throw new Exception($"Api request returned unexpected status code {response.StatusCode} for {requestUrl}");
                    }
                }

                _logger.LogInformation($"[RAWG] Update local game data success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[RAWG] Update local game data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task CacheGameDataAsync(string jsonRaw)
        {
            var batch = new List<RawgGame>();
            int batchSize = 1000;

            JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonRaw);
            var resultTokens = responseObject.GetValue("results")?.ToList() ?? new List<JToken>();

            foreach (JToken token in resultTokens)
            {
                try
                {
                    var rawgGame = token.ToObject<RawgGame>();
                    batch.Add(rawgGame);

                    if (batch.Count >= batchSize)
                    {
                        await _gamesRepository.AddOrUpdateRangeAsync(batch).ConfigureAwait(false);
                        batch.Clear();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[RAWG] {ex.Message} - {ex.StackTrace}");
                }
            }

            if (batch.Any())
                await _gamesRepository.AddOrUpdateRangeAsync(batch).ConfigureAwait(false);
        }

        private async Task ImportGameDataAsync_DEBUG()
        {
            var stopwatch = Stopwatch.StartNew();
            var rawgGames = new List<RawgGame>();

            int batchSize = 1000;

            var dataFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "RAWG_Data")).ToList();
            foreach (string filepath in dataFiles)
            {
                string jsonRaw = File.ReadAllText(filepath);
                JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonRaw);

                var resultTokens = responseObject.GetValue("results")?.ToList() ?? new List<JToken>();

                foreach (JToken token in resultTokens)
                {
                    try
                    {
                        var rawgGame = token.ToObject<RawgGame>();
                        rawgGames.Add(rawgGame);

                        if (rawgGames.Count >= batchSize)
                        {
                            await _gamesRepository.AddOrUpdateRangeAsync(rawgGames).ConfigureAwait(false);
                            rawgGames.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[RAWG] {ex.Message} - {ex.StackTrace}");
                    }
                }
            }

            await _gamesRepository.AddOrUpdateRangeAsync(rawgGames).ConfigureAwait(false);

            stopwatch.Stop();
        }
        #endregion Methods..
    }
}
