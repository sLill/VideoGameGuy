using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using VideoGameGuy.Configuration;
using VideoGameGuy.Data;
using System.Text;
using Microsoft.SqlServer.Server;
using Azure.Core;

namespace VideoGameGuy.Core
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

            var token = await GetAccessTokenAsync();
            await UpdateLocalGameDataAsync(token, updatedOnStartDate, updatedOnEndDate).ConfigureAwait(false);

            stopwatch.Stop();
            _logger.LogInformation($"[IGDB] Update complete. Total time: {stopwatch.ElapsedMilliseconds / 1000} seconds");
        }

        private async Task<IgdbAccessToken> GetAccessTokenAsync()
        {
            IgdbAccessToken token = null;

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
                            token = JsonConvert.DeserializeObject<IgdbAccessToken>(responseString);
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

        private async Task UpdateLocalGameDataAsync(IgdbAccessToken token, DateTime? updatedOnStartDate, DateTime updatedOnEndDate)
        {
            try
            {
                _logger.LogInformation($"[IGDB] Updating local game data..");

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);
                    httpClient.DefaultRequestHeaders.Add("Client-ID", _settings.Value.ClientId);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.access_token}");

                    int offset = 0;
                    int retries = 0;
                    int retryLimit = _settings.Value.RetryLimit;
                    int rateLimit = _settings.Value.RateLimit;
                    int resultsPerRequest = 500;
                    int requestsThisSecond = 0;
                    string requestUri = $"{_settings.Value.ApiUrl}/{IgdbEndpoints.Games}";

                    while (true)
                    {
                        string query = $"fields *;" +
                                       $"limit {resultsPerRequest};"+
                                       $"offset {offset};";

                        var content = new StringContent(query, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            
                            // Parse main data
                            List<IgdbGame> games = ParseGameData(responseString);

                            // Get and parse relational data
                            List<long> artworkIds = games.SelectMany(x => x.artworks).Distinct()?.ToList() ?? new List<long>(); 

                            offset += resultsPerRequest;
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

                _logger.LogInformation($"[IGDB] Update local game data success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IGDB] Update local game data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        private List<IgdbGame> ParseGameData(string jsonRaw)
        {
            List<IgdbGame> games = new List<IgdbGame>();

            // Deserialize games
            JArray gameObjectArray = JsonConvert.DeserializeObject<JArray>(jsonRaw);
            foreach (JToken gameToken in gameObjectArray)
            {
                IgdbGame game = gameToken.ToObject<IgdbGame>();
                games.Add(game);
            }
            
            return games;
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
