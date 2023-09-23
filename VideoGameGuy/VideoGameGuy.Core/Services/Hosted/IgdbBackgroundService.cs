using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using VideoGameGuy.Configuration;
using VideoGameGuy.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VideoGameGuy.Core
{
    public class IgdbBackgroundService : BackgroundService
    {
        #region Fields..
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<IgdbBackgroundService> _logger;
        private readonly IOptions<IgdbApiSettings> _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        private ISystemStatusRepository _systemStatusRepository;
        private IIgdbGamesRepository _igdbGamesRepository;

        private Stopwatch _requestRateTimer;
        private int _requestsThisSecond;
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

            Initialize();
        }
        #endregion Constructors..

        #region Methods..
        private void Initialize()
        {
            var serviceScrope = _serviceProvider.CreateScope();
            _systemStatusRepository = serviceScrope.ServiceProvider.GetRequiredService<ISystemStatusRepository>();
            _igdbGamesRepository = serviceScrope.ServiceProvider.GetRequiredService<IIgdbGamesRepository>();
        }

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

                    //await ImportGameDataAsync_DEBUG();
                    await PollAndCacheAsync(startDate);

                    currentSystemStatus.Igdb_UpdatedOnUtc = DateTime.UtcNow;
                    await _systemStatusRepository.UpdateAsync(currentSystemStatus);
                }

                await Task.Delay(TimeSpan.FromHours(6), cancellationToken);
            }
        }

        private async Task PollAndCacheAsync(DateTime? updatedOnStartDate)
        {
            _logger.LogInformation($"[IGDB] Beginning update..");
            var stopwatch = Stopwatch.StartNew();

            var token = await GetAccessTokenAsync();
            await UpdateGameDataAsync(token, updatedOnStartDate);

            stopwatch.Stop();
            _logger.LogInformation($"[IGDB] Update complete. Total time: {stopwatch.ElapsedMilliseconds / 1000} seconds");
        }

        private async Task<IgdbApiAccessToken> GetAccessTokenAsync()
        {
            IgdbApiAccessToken token = null;

            try
            {
                _logger.LogInformation($"[IGDB] Authenticating..");

                int retryLimit = _settings.Value.RetryLimit;
                int retries = 0;

                while (true)
                {
                    using (var httpClient = _httpClientFactory.CreateClient())
                    {
                        httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);

                        string requestUri = _settings.Value.AuthUrl;
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("client_id",_settings.Value.ClientId),
                            new KeyValuePair<string, string>("client_secret", _settings.Value.ClientSecret),
                            new KeyValuePair<string, string>("grant_type", "client_credentials")
                        });

                        var response = await httpClient.PostAsync(requestUri, content);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseString = await response.Content.ReadAsStringAsync();
                            token = JsonConvert.DeserializeObject<IgdbApiAccessToken>(responseString);
                            break;
                        }

                        // Retry on timeouts
                        else if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
                        {
                            if (retries >= retryLimit)
                            {
                                _logger.LogError($"[IGDB] Request retry limit exceeded for {requestUri}. Aborting.");
                                retries = 0;
                                break;
                            }
                            else
                            {
                                _logger.LogWarning($"[IGDB] Request timed out {requestUri}. Retrying ({retries}/{retryLimit})");
                                retries++;
                            }
                        }
                        else
                            throw new Exception($"Api request returned unexpected status code {response.StatusCode} for {requestUri}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IGDB] Get access token failed. {ex.Message} - {ex.StackTrace}");
            }

            if (token == null)
                _logger.LogInformation("[IGDB] Could not retrieve access token.");
            else
                _logger.LogInformation("[IGDB] Access token received.");

            return token;
        }

        private async Task UpdateGameDataAsync(IgdbApiAccessToken token, DateTime? updatedOnStartDate)
        {
            try
            {
                _logger.LogInformation($"[IGDB] Updating game data..");

                // Loop until no more data is returned
                while (true)
                {
                    int offset = 0;
                    int resultsPerRequest = 500;

                    string requestUri = $"{_settings.Value.ApiUrl}/{IgdbApiEndpoints.Games}";
                    long unixStartTime = updatedOnStartDate.HasValue ? ((DateTimeOffset)updatedOnStartDate.Value).ToUnixTimeSeconds() : 0;
                    
                    string query = $"fields *;" +
                                   $"limit {resultsPerRequest};" +
                                   $"offset {offset};" +
                                   $"where updated_at >= {unixStartTime}";

                    string gameJson = await ExecuteRequestAsync(token, requestUri, query);
                    if (gameJson != null)
                    {
                        List<IgdbApiGame> games = ParseJsonArray<IgdbApiGame>(gameJson);

                        // Get and parse relational data
                        await UpdateArtworkDataAsync(token, games, unixStartTime);

                        List<long> gameModeIds = games.SelectMany(x => x.game_modes).Distinct()?.ToList() ?? new List<long>();
                        List<long> multiplayerModeIds = games.SelectMany(x => x.multiplayer_modes).Distinct()?.ToList() ?? new List<long>();
                        List<long> platformIds = games.SelectMany(x => x.platforms).Distinct()?.ToList() ?? new List<long>();
                        List<long> screenshotIds = games.SelectMany(x => x.screenshots).Distinct()?.ToList() ?? new List<long>();
                        List<long> themeIds = games.SelectMany(x => x.themes).Distinct()?.ToList() ?? new List<long>();

         


                        // If the number of games return does not match the number of games requested, then we have reached the end
                        if (games.Count == resultsPerRequest)
                            offset += resultsPerRequest;
                        else
                            break;
                    }

                    // Whether it's a timeout or an error, take a break for a awhile
                    else
                        break;
                }

                _logger.LogInformation($"[IGDB] Update game data complete");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IGDB] Update game data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task UpdateArtworkDataAsync(IgdbApiAccessToken token, List<IgdbApiGame> games, long unixStartTime)
        {
            string requestUri = $"{_settings.Value.ApiUrl}/{IgdbApiEndpoints.Artworks}";
            List<long> artworkIds = games.SelectMany(x => x.artworks).Distinct()?.ToList() ?? new List<long>();
         
            int offset = 0;
            int resultsPerRequest = 500;

            while (true)
            {
                foreach (long id in artworkIds)
                {
                    string query = $"fields *;" +
                                   $"limit {resultsPerRequest};" +
                                   $"offset {offset};" +
                                   $"where updated_at >= {unixStartTime}";

                    string artworkJson = await ExecuteRequestAsync(token, requestUri, query);
                    if (artworkJson != null)
                    {
                        List<IgdbApiArtwork> artworks = ParseJsonArray<IgdbApiArtwork>(artworkJson);



                        // Stop looping once we receive fewer than the number of results that were requested
                        if (artworks.Count >= resultsPerRequest)
                            offset += 500;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
        }

        private async Task<string> ExecuteRequestAsync(IgdbApiAccessToken token, string requestUri, string query)
        {
            string requestResponse = null;

            int retries = 0;
            int retryLimit = _settings.Value.RetryLimit;
            _requestRateTimer = _requestRateTimer ?? Stopwatch.StartNew();

            // Limit the number of requests per second
            if (_requestsThisSecond >= _settings.Value.RateLimit)
            {
                // Wait out the rest of the second before allowing requests to continue
                int timeRemaining = 1000 - (int)_requestRateTimer.ElapsedMilliseconds;
                if (timeRemaining > 0)
                    await Task.Delay(timeRemaining);
                
                _requestRateTimer.Restart();
            }

            _requestsThisSecond++;

            while (requestResponse != null)
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);
                    httpClient.DefaultRequestHeaders.Add("Client-ID", _settings.Value.ClientId);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.access_token}");
                    
                    var content = new StringContent(query, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(requestUri, content);

                    if (response.IsSuccessStatusCode)
                        requestResponse = await response.Content.ReadAsStringAsync();

                    // Retry on timeouts
                    else if (response.StatusCode == HttpStatusCode.RequestTimeout || response.StatusCode == HttpStatusCode.GatewayTimeout)
                    {
                        if (retries >= retryLimit)
                        {
                            _logger.LogError($"[IGDB] Request retry limit exceeded for {requestUri}. Aborting.");
                            retries = 0;
                            break;
                        }
                        else
                        {
                            _logger.LogWarning($"[IGDB] Request timed out {requestUri}. Retrying ({retries}/{retryLimit})");
                            retries++;
                        }
                    }
                    else
                        throw new Exception($"Api request returned unexpected status code {response.StatusCode} for {requestUri}");
                }
            }

            return requestResponse;
        }

        private List<T> ParseJsonArray<T>(string jsonRaw) where T : class 
        {
            List<T> results = new List<T>();

            // Deserialize 
            JArray objectArray = JsonConvert.DeserializeObject<JArray>(jsonRaw);
            foreach (JToken token in objectArray)
            {
                T tokenObject = token.ToObject<T>();
                results.Add(tokenObject);
            }

            return results;
        }

        private async Task CacheGameDataAsync(List<IgdbApiArtwork> artworks)
        {
            //var batch = new List<RawgGame>();

            //JObject responseObject = JsonConvert.DeserializeObject<JObject>(jsonRaw);
            //var resultTokens = responseObject.GetValue("results")?.ToList() ?? new List<JToken>();

            //foreach (JToken token in resultTokens)
            //{
            //    try
            //    {
            //        var rawgGame = token.ToObject<RawgGame>();
            //        batch.Add(rawgGame);

            //        if (batch.Count >= batchSize)
            //        {
            //            await _igdbGamesRepository.AddOrUpdateRangeAsync(batch);
            //            batch.Clear();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError($"[IGDB] {ex.Message} - {ex.StackTrace}");
            //    }
            //}

            //if (batch.Any())
            //    await _igdbGamesRepository.AddOrUpdateRangeAsync(batch);
        }
        #endregion Methods..
    }
}
