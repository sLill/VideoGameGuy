using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using VideoGameGuy.Configuration;
using VideoGameGuy.Data;

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
        private IIgdbArtworksRepository _igdbArtworksRepository;
        private IIgdbGameModesRepository _igdbGameModesRepository;
        private IIgdbGamesRepository _igdbGamesRepository;
        private IIgdbMultiplayerModesRepository _igdbMultiplayerModesRepository;
        private IIgdbPlatformFamiliesRepository _igdbPlatformFamiliesRepository;
        private IIgdbPlatformLogosRepository _igdbPlatformLogosRepository;
        private IIgdbPlatformsRepository _igdbPlatformsRepository;
        private IIgdbScreenshotsRepository _igdbScreenshotsRepository;
        private IIgdbThemesRepository _igdbThemesRepository;

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
            var serviceScope = _serviceProvider.CreateScope();

            _systemStatusRepository = serviceScope.ServiceProvider.GetRequiredService<ISystemStatusRepository>();
            _igdbArtworksRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbArtworksRepository>();
            _igdbGameModesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGameModesRepository>();
            _igdbGamesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGamesRepository>();
            _igdbMultiplayerModesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbMultiplayerModesRepository>();
            _igdbPlatformFamiliesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbPlatformFamiliesRepository>();
            _igdbPlatformLogosRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbPlatformLogosRepository>();
            _igdbPlatformsRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbPlatformsRepository>();
            _igdbScreenshotsRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbScreenshotsRepository>();
            _igdbThemesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbThemesRepository>();
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
            await UpdateDataAsync(token, updatedOnStartDate);

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

        private async Task UpdateDataAsync(IgdbApiAccessToken token, DateTime? updatedOnStartDate)
        {
            await UpdateEndpointDataAsync<IgdbApiGame>(token, IgdbApiEndpoints.Games,  updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiArtwork>(token, IgdbApiEndpoints.Artworks,  updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiGameMode>(token, IgdbApiEndpoints.GameModes,  updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiMultiplayerMode>(token, IgdbApiEndpoints.MultiplayerModes, updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiPlatform>(token, IgdbApiEndpoints.Platforms,  updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiPlatformLogo>(token, IgdbApiEndpoints.PlatformLogos,  updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiPlatformFamily>(token, IgdbApiEndpoints.PlatformFamilies,  updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiScreenshot>(token, IgdbApiEndpoints.Screenshots, updatedOnStartDate);
            await UpdateEndpointDataAsync<IgdbApiTheme>(token, IgdbApiEndpoints.Themes,  updatedOnStartDate);
        }

        private async Task UpdateEndpointDataAsync<T>(IgdbApiAccessToken token, string endpoint, DateTime? updatedOnStartDate) where T : class 
        {
            Type apiObjectType = typeof(T);

            try
            {
                _logger.LogInformation($"[IGDB] Updating {apiObjectType} data..");

                // Loop until no more data is returned
                while (true)
                {
                    int offset = 0;
                    int resultsPerRequest = 500;

                    string requestUri = $"{_settings.Value.ApiUrl}/{endpoint}";
                    long unixStartTime = updatedOnStartDate.HasValue ? ((DateTimeOffset)updatedOnStartDate.Value).ToUnixTimeSeconds() : 0;

                    string query = $"fields *;" +
                                   $"limit {resultsPerRequest};" +
                                   $"offset {offset};" +
                                   $"where updated_at >= {unixStartTime}";

                    string responseJson = await ExecuteRequestAsync(token, requestUri, query);
                    if (responseJson != null)
                    {
                        List<T> apiObjects = ParseJsonArray<T>(responseJson);
                        if (apiObjects?.Any() ?? false)
                            await SaveModelDataAsync(apiObjects);

                        // If the number of items returned does not match the number of items requested, then we have reached the end
                        if (apiObjects.Count() == resultsPerRequest)
                            offset += resultsPerRequest;
                        else
                            break;
                    }

                    // Whether it's a timeout or an error, take a break for a awhile
                    else
                        break;
                }

                _logger.LogInformation($"[IGDB] Update {apiObjectType} data complete");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IGDB] Update {apiObjectType} data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task SaveModelDataAsync<U>(IEnumerable<U> modelObjects) where U : class
        {
            if (modelObjects?.Any() ?? false)
            {
                switch (modelObjects)
                {
                    case IEnumerable<IgdbApiArtwork> apiArtworks:
                        await _igdbArtworksRepository.AddOrUpdateRangeAsync(apiArtworks);
                        break;
                    case IEnumerable<IgdbApiGameMode> apiGameModes:
                        await _igdbGameModesRepository.AddOrUpdateRangeAsync(apiGameModes);
                        break;
                    case IEnumerable<IgdbApiGame> apiGames:
                        await _igdbGamesRepository.AddOrUpdateRangeAsync(apiGames);
                        break;
                    case IEnumerable<IgdbApiMultiplayerMode> apiMultiplayerModes:
                        await _igdbMultiplayerModesRepository.AddOrUpdateRangeAsync(apiMultiplayerModes);
                        break;
                    case IEnumerable<IgdbApiPlatformFamily> apiPlatformFamilies:
                        await _igdbPlatformFamiliesRepository.AddOrUpdateRangeAsync(apiPlatformFamilies);
                        break;
                    case IEnumerable<IgdbApiPlatformLogo> apiPlatformLogos:
                        await _igdbPlatformLogosRepository.AddOrUpdateRangeAsync(apiPlatformLogos);
                        break;
                    case IEnumerable<IgdbApiPlatform> apiPlatforms:
                        await _igdbPlatformsRepository.AddOrUpdateRangeAsync(apiPlatforms);
                        break;
                    case IEnumerable<IgdbApiScreenshot> apiScreenshots:
                        await _igdbScreenshotsRepository.AddOrUpdateRangeAsync(apiScreenshots);
                        break;
                    case IEnumerable<IgdbApiTheme> apiThemes:
                        await _igdbThemesRepository.AddOrUpdateRangeAsync(apiThemes);
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
        #endregion Methods..
    }
}
