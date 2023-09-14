using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using VideoGameShowdown.Common;
using VideoGameShowdown.Configuration;
using VideoGameShowdown.Data;

namespace VideoGameShowdown.Core
{
    public class RawgBackgroundService : BackgroundService
    {
        #region Fields..
        private readonly ILogger<RawgBackgroundService> _logger;
        private readonly IOptions<RawgApiSettings> _settings;
        private readonly ApplicationDbContext _applicationDbContext;
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
                                     ApplicationDbContext applicationDbContext,
                                     IGamesRepository gamesRepository,
                                     IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _settings = settings;
            _applicationDbContext = applicationDbContext;
            _gamesRepository = gamesRepository;
            _httpClientFactory = httpClientFactory;
        }
        #endregion Constructors..

        #region Methods..
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ImportGameDataAsync();
                //await PollAndCacheAsync();

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }

        private async Task PollAndCacheAsync()
        {
            _logger.LogInformation($"[RAWG] Beginning update..");

            await UpdateLocalGameDataAsync().ConfigureAwait(false);

            _logger.LogInformation($"[RAWG] Update complete");
        }

        private async Task UpdateLocalGameDataAsync()
        {
            try
            {
                _logger.LogInformation($"[RAWG] Updating local game data..");

                Directory.CreateDirectory(LocalCachePath);

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);

                    string baseUrl = Path.Combine(_settings.Value.ApiUrl, _settings.Value.Endpoints["Games"]);
                    string requestUrl = $"{baseUrl}?key={_settings.Value.ApiKey}";
                    int pageSize = _settings.Value.Response_PageSize;

                    int pageNumber = 1;
                    int retryLimit = _settings.Value.RetryLimit;
                    int retries = 0;

                    while (true)
                    {
                        string requestUrlPaged = $"{requestUrl}&page={pageNumber}&page_size={pageSize}";
                        string localFilepath = Path.Combine(LocalCachePath, $"games_{pageNumber}.json");

                        // Request and cache file if it is old or does not exist
                        if (!File.Exists(localFilepath) || (int)(DateTime.Now - File.GetCreationTime(localFilepath)).TotalDays >= _settings.Value.LocalCache_PollingInterval_Days)
                        {
                            var response = await httpClient.GetAsync(requestUrlPaged);
                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();

                                // Cache response to local file
                                File.WriteAllText(localFilepath, responseContent);

                                // Check for next page
                                var jsonObject = JsonConvert.DeserializeObject<JObject>(responseContent);
                                var nextToken = jsonObject["next"];

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
                                    _logger.LogError($"[RAWG] Request retry limit exceeded for {requestUrlPaged}. Aborting.");

                                    retries = 0;
                                    pageNumber++;
                                }
                                else
                                {
                                    _logger.LogWarning($"[RAWG] Request timed out {requestUrlPaged}. Retrying ({retries}/{retryLimit})");
                                    retries++;
                                }
                            }

                            // End of data
                            else if (response.StatusCode == HttpStatusCode.BadGateway)
                                break;

                            else
                                throw new Exception($"Api request returned unexpected status code {response.StatusCode} for {requestUrlPaged}");
                        }
                        else
                            pageNumber++;
                    }
                }

                _logger.LogInformation($"[RAWG] Update local game data success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[RAWG] Update local game data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task ImportGameDataAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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
                        _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                    }
                }
            }

            await _gamesRepository.AddOrUpdateRangeAsync(rawgGames).ConfigureAwait(false);

            stopwatch.Stop();
        }
        #endregion Methods..
    }
}
