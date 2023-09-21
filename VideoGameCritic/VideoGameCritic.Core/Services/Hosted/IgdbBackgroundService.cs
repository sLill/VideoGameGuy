using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using VideoGameCritic.Configuration;
using VideoGameCritic.Data;

namespace VideoGameCritic.Core
{
    public class IgdbBackgroundService : BackgroundService
    {
        #region Fields..
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IgdbBackgroundService> _logger;
        private readonly IOptions<IgdbApiSettings> _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ISystemStatusRepository _systemStatusRepository;
        private readonly IIgdbGamesRepository _igdbGamesRepository;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbBackgroundService(IServiceProvider serviceProvider,
                                     ILogger<IgdbBackgroundService> logger,
                                     IOptions<IgdbApiSettings> settings,
                                     IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings;
            _httpClientFactory = httpClientFactory;

            var serviceScrope = _serviceProvider.CreateScope();
            _systemStatusRepository = serviceScrope.ServiceProvider.GetRequiredService<ISystemStatusRepository>();
            _igdbGamesRepository = serviceScrope.ServiceProvider.GetRequiredService<IIgdbGamesRepository>();
        }
        #endregion Constructors..

        #region Methods..
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var currentSystemStatus = await _systemStatusRepository.GetCurrentStatusAsync();

                // Pull and cache data if it has never been done before or if the polling period has elapsed
                if (currentSystemStatus.Igdb_UpdatedOnUtc == default
                    || (DateTime.UtcNow - currentSystemStatus.Igdb_UpdatedOnUtc.Value).TotalDays >= _settings.Value.LocalCache_UpdateInterval_Days)
                {
                    // To avoid timezone issues, move the start date a day back
                    DateTime startDate = currentSystemStatus.Igdb_UpdatedOnUtc == default 
                        ? DateTime.MinValue : (currentSystemStatus.Igdb_UpdatedOnUtc.Value.Date - TimeSpan.FromDays(1));

                    DateTime endDate = DateTime.UtcNow.Date;

                    //await ImportGameDataAsync_DEBUG();
                    await PollAndCacheAsync(startDate, endDate);

                    currentSystemStatus.Igdb_UpdatedOnUtc = DateTime.UtcNow;
                    await _systemStatusRepository.UpdateAsync(currentSystemStatus);
                }

                await Task.Delay(TimeSpan.FromHours(6), cancellationToken);
            }
        }

        private async Task PollAndCacheAsync(DateTime? updatedOnStartDate, DateTime updatedOnEndDate)
        {
            _logger.LogInformation($"[IGDB] Beginning update..");
            var stopwatch = Stopwatch.StartNew();

            await UpdateLocalGameDataAsync(updatedOnStartDate, updatedOnEndDate).ConfigureAwait(false);

            stopwatch.Stop();
            _logger.LogInformation($"[IGDB] Update complete. Total time: {stopwatch.ElapsedMilliseconds / 1000} seconds");
        }

        private async Task UpdateLocalGameDataAsync(DateTime? updatedOnStartDate, DateTime updatedOnEndDate)
        {
            try
            {
                _logger.LogInformation($"[IGDB] Updating local game data..");

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);

                    //int pageNumber = 1;
                    int retryLimit = _settings.Value.RetryLimit;
                    int retries = 0;

                    while (true)
                    {
                        break;
                        //string baseUrl = Path.Combine(_settings.Value.ApiUrl, _settings.Value.Endpoints["Games"]);
                        //string requestUrl = $"{baseUrl}?key={_settings.Value.ApiKey}&page={pageNumber}&page_size={_settings.Value.Response_PageSize}";

                        //// Filter by updated date
                        //if (updatedOnStartDate != null)
                        //    requestUrl = $"{requestUrl}&updated={updatedOnStartDate.Value.ToString("yyyy-MM-dd")},{updatedOnEndDate.ToString("yyyy-MM-dd")}";

                        //// Request and cache file if it is old or does not exist
                        //var response = await httpClient.GetAsync(requestUrl);
                        //if (response.IsSuccessStatusCode)
                        //{
                        //    // Cache response to db
                        //    string responseString = await response.Content.ReadAsStringAsync();
                        //    await CacheGameDataAsync(responseString).ConfigureAwait(false);

                        //    // Check for next page
                        //    JObject json = JsonConvert.DeserializeObject<JObject>(responseString);

                        //    var nextToken = json["next"];
                        //    if (nextToken.Type == JTokenType.Null)
                        //        break;
                        //    else
                        //    {
                        //        retries = 0;
                        //        pageNumber++;
                        //    }
                        //}

                        //// Retry on timeouts
                        //else if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
                        //{
                        //    if (retries >= retryLimit)
                        //    {
                        //        _logger.LogError($"[IGDB] Request retry limit exceeded for {requestUrl}. Aborting.");

                        //        retries = 0;
                        //        pageNumber++;
                        //    }
                        //    else
                        //    {
                        //        _logger.LogWarning($"[IGDB] Request timed out {requestUrl}. Retrying ({retries}/{retryLimit})");
                        //        retries++;
                        //    }
                        //}

                        //// End of data
                        //else if (response.StatusCode == HttpStatusCode.BadGateway)
                        //    break;

                        //else
                        //    throw new Exception($"Api request returned unexpected status code {response.StatusCode} for {requestUrl}");
                    }
                }

                _logger.LogInformation($"[IGDB] Update local game data success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IGDB] Update local game data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        //private async Task CacheGameDataAsync(string jsonRaw)
        //{
        //    var batch = new List<RawgGame>();
        //    int batchSize = 1000;

        //    JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonRaw);
        //    var resultTokens = responseObject.GetValue("results")?.ToList() ?? new List<JToken>();

        //    foreach (JToken token in resultTokens)
        //    {
        //        try
        //        {
        //            var rawgGame = token.ToObject<RawgGame>();
        //            batch.Add(rawgGame);

        //            if (batch.Count >= batchSize)
        //            {
        //                await _igdbGamesRepository.AddOrUpdateRangeAsync(batch).ConfigureAwait(false);
        //                batch.Clear();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError($"[IGDB] {ex.Message} - {ex.StackTrace}");
        //        }
        //    }

        //    if (batch.Any())
        //        await _igdbGamesRepository.AddOrUpdateRangeAsync(batch).ConfigureAwait(false);
        //}

        //private async Task ImportGameDataAsync_DEBUG()
        //{
        //    var stopwatch = Stopwatch.StartNew();
        //    var rawgGames = new List<RawgGame>();

        //    int batchSize = 1000;

        //    var dataFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "RAWG_Data")).ToList();
        //    foreach (string filepath in dataFiles)
        //    {
        //        string jsonRaw = File.ReadAllText(filepath);
        //        JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonRaw);

        //        var resultTokens = responseObject.GetValue("results")?.ToList() ?? new List<JToken>();

        //        foreach (JToken token in resultTokens)
        //        {
        //            try
        //            {
        //                var rawgGame = token.ToObject<RawgGame>();
        //                rawgGames.Add(rawgGame);

        //                if (rawgGames.Count >= batchSize)
        //                {
        //                    await _igdbGamesRepository.AddOrUpdateRangeAsync(rawgGames).ConfigureAwait(false);
        //                    rawgGames.Clear();
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError($"[IGDB] {ex.Message} - {ex.StackTrace}");
        //            }
        //        }
        //    }

        //    await _igdbGamesRepository.AddOrUpdateRangeAsync(rawgGames).ConfigureAwait(false);

        //    stopwatch.Stop();
        //}
        #endregion Methods..
    }
}
