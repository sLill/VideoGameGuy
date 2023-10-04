using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using VideoGameGuy.Configuration;
using VideoGameGuy.Data;

namespace VideoGameGuy.Core
{
    public class RawgBackgroundService : BackgroundService
    {
        #region Fields..
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RawgBackgroundService> _logger;
        private readonly IOptions<RawgApiSettings> _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ISystemStatusRepository _systemStatusRepository;
        private readonly IRawgGamesRepository _rawgGamesRepository;
        #endregion Fields..

        #region Properties..
        private string LocalCachePath
            => Path.Combine(AppContext.BaseDirectory, _settings.Value.LocalCache_RelativePath);
        #endregion Properties..

        #region Constructors..
        public RawgBackgroundService(IServiceProvider serviceProvider,
                                     ILogger<RawgBackgroundService> logger,
                                     IOptions<RawgApiSettings> settings,
                                     IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings;
            _httpClientFactory = httpClientFactory;

            var serviceScrope = _serviceProvider.CreateScope();
            _systemStatusRepository = serviceScrope.ServiceProvider.GetRequiredService<ISystemStatusRepository>();
            _rawgGamesRepository = serviceScrope.ServiceProvider.GetRequiredService<IRawgGamesRepository>();

        }
        #endregion Constructors..

        #region Methods..
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var currentSystemStatus = await _systemStatusRepository.GetCurrentStatusAsync();
                
                // Pull and cache data if it has never been done before or if the polling period has elapsed
                if (currentSystemStatus.Rawg_UpdatedOnUtc == default 
                    || (DateTime.UtcNow - currentSystemStatus.Rawg_UpdatedOnUtc.Value).TotalDays >= _settings.Value.LocalCache_UpdateInterval_Days)
                {
                    // To avoid any overlap issues, move the start date a day back
                    DateTime startDate = currentSystemStatus.Rawg_UpdatedOnUtc == default
                        ? DateTime.MinValue : (currentSystemStatus.Rawg_UpdatedOnUtc.Value.Date - TimeSpan.FromDays(1));

                    DateTime endDate = DateTime.UtcNow.Date;

                    //await PollAndCacheAsync(startDate, endDate);

                    currentSystemStatus.Rawg_UpdatedOnUtc = DateTime.UtcNow;
                    await _systemStatusRepository.UpdateAsync(currentSystemStatus);
                }

                await Task.Delay(TimeSpan.FromHours(6), cancellationToken);
            }
        }

        private async Task PollAndCacheAsync(DateTime? updatedOnStartDate, DateTime updatedOnEndDate)
        {
            _logger.LogInformation($"[RAWG] Beginning update..");
            var stopwatch = Stopwatch.StartNew();

            await UpdateGameDataAsync(updatedOnStartDate, updatedOnEndDate);

            stopwatch.Stop();
            _logger.LogInformation($"[RAWG] Update complete. Total time: {stopwatch.ElapsedMilliseconds / 1000} seconds");
        }

        private async Task UpdateGameDataAsync(DateTime? updatedOnStartDate, DateTime updatedOnEndDate)
        {
            try
            {
                _logger.LogInformation($"[RAWG] Updating game data..");

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);

                    int pageNumber = 1;
                    int retryLimit = _settings.Value.RetryLimit;
                    int retries = 0;

                    while (true)
                    {
                        string baseUrl = Path.Combine(_settings.Value.ApiUrl, RawgApiEndpoints.Games);
                        string requestUrl = $"{baseUrl}?key={_settings.Value.ApiKey}&page={pageNumber}&page_size={_settings.Value.Response_PageSize}";

                        // Filter by updated date
                        if (updatedOnStartDate != null)
                            requestUrl = $"{requestUrl}&updated={updatedOnStartDate.Value.ToString("yyyy-MM-dd")},{updatedOnEndDate.ToString("yyyy-MM-dd")}";   

                        // Request and cache file if it is old or does not exist
                        var response = await httpClient.GetAsync(requestUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            // Cache response to db
                            string responseString = await response.Content.ReadAsStringAsync();
                            await CacheGameDataAsync(responseString);

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

                _logger.LogInformation($"[RAWG] Update game data success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[RAWG] Update game data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task CacheGameDataAsync(string jsonRaw)
        {
            var batch = new List<RawgApiGame>();
            int batchSize = 1000;

            JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonRaw);
            var resultTokens = responseObject.GetValue("results")?.ToList() ?? new List<JToken>();

            foreach (JToken token in resultTokens)
            {
                try
                {
                    var rawgGame = token.ToObject<RawgApiGame>();
                    batch.Add(rawgGame);

                    if (batch.Count >= batchSize)
                    {
                        await _rawgGamesRepository.AddOrUpdateRangeAsync(batch);
                        batch.Clear();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[RAWG] {ex.Message} - {ex.StackTrace}");
                }
            }

            if (batch.Any())
                await _rawgGamesRepository.AddOrUpdateRangeAsync(batch);
        }
        #endregion Methods..
    }
}
