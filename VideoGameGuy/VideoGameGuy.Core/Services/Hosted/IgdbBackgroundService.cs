using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syncfusion.EJ2.Linq;
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

        private IIgdbGames_GameModesRepository _igdbGames_GameModesRepository;
        private IIgdbGames_MultiplayerModesRepository _igdbGames_MultiplayerModesRepository;
        private IIgdbGames_PlatformsRepository _igdbGames_PlatformsRepository;
        private IIgdbGames_ThemesRepository _igdbGames_ThemesRepository;
        private IIgdbGames_ArtworksRepository _igdbGames_ArtworksRepository;
        private IIgdbGames_ScreenshotsRepository _igdbGames_ScreenshotsRepository;
        private IIgdbPlatforms_PlatformFamiliesRepository _iIgdbPlatforms_PlatformFamiliesRepository;
        private IIgdbPlatforms_PlatformLogosRepository _iIgdbPlatforms_PlatformLogosRepository;
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

            _igdbGames_GameModesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGames_GameModesRepository>();
            _igdbGames_MultiplayerModesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGames_MultiplayerModesRepository>();
            _igdbGames_PlatformsRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGames_PlatformsRepository>();
            _igdbGames_ThemesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGames_ThemesRepository>();
            _igdbGames_ArtworksRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGames_ArtworksRepository>();
            _igdbGames_ScreenshotsRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbGames_ScreenshotsRepository>();
            _iIgdbPlatforms_PlatformFamiliesRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbPlatforms_PlatformFamiliesRepository>();
            _iIgdbPlatforms_PlatformLogosRepository = serviceScope.ServiceProvider.GetRequiredService<IIgdbPlatforms_PlatformLogosRepository>();
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
            await UpdateEndpointDataAsync<IgdbApiGame>(token, IgdbApiEndpoints.Games,  updatedOnStartDate, IgdbApiEndpoints.GameModes, IgdbApiEndpoints.MultiplayerModes, IgdbApiEndpoints.Platforms, IgdbApiEndpoints.Themes);
            await UpdateEndpointDataAsync<IgdbApiPlatform>(token, IgdbApiEndpoints.Platforms,  updatedOnStartDate, IgdbApiEndpoints.PlatformFamilies, IgdbApiEndpoints.PlatformLogos);
        }

        private async Task UpdateEndpointDataAsync<T>(IgdbApiAccessToken token, string endpoint, DateTime? updatedOnStartDate, params string[] subEndpoints) where T : class 
        {
            Type apiObjectType = typeof(T);

            try
            {
                _logger.LogInformation($"[IGDB] Updating {apiObjectType} data..");

                Stopwatch requestRateTimer = Stopwatch.StartNew();
                int requestsThisSecond = 0;

                int offset = 0;
                int resultsPerRequest = 500;

                // Loop until no more data is returned
                while (true)
                {
                    string requestUri = $"{_settings.Value.ApiUrl}/{endpoint}";
                    long unixStartTime = updatedOnStartDate != default ? ((DateTimeOffset)updatedOnStartDate.Value).ToUnixTimeSeconds() : 0;

                    // Build query
                    StringBuilder query = new StringBuilder($"fields *");

                    if (subEndpoints?.Any() ?? false)
                        query.Append(string.Join(',', subEndpoints.Select(x => $"{x}.*")));

                    query.Append(';');

                    if (updatedOnStartDate != null)
                        query.Append($"where updated_at >= {unixStartTime};");

                    query.Append($"limit {resultsPerRequest};" +
                                 $"offset {offset};");

                    // Limit the number of requests per second
                    if (requestsThisSecond >= _settings.Value.RateLimit || requestRateTimer.ElapsedMilliseconds >= 1000)
                    {
                        // Wait out the rest of the second before allowing requests to continue
                        int timeRemaining = 1000 - (int)requestRateTimer.ElapsedMilliseconds;
                        if (timeRemaining > 0)
                            await Task.Delay(timeRemaining);

                        requestRateTimer.Restart();
                        requestsThisSecond = 0;
                    }

                    requestsThisSecond++;

                    string responseJson = await ExecuteRequestAsync(token, requestUri, query.ToString());
                    if (responseJson != null)
                    {
                        List<T> apiObjects = ParseJsonArray<T>(responseJson);
                        if (apiObjects?.Any() ?? false)
                            await SaveEndpointDataAsync(apiObjects);

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

        private async Task SaveEndpointDataAsync<U>(IEnumerable<U> modelObjects) where U : class
        {
            if (modelObjects?.Any() ?? false)
            {
                switch (modelObjects)
                {
                    case IEnumerable<IgdbApiArtwork> apiArtworks:
                        await _igdbArtworksRepository.AddOrUpdateRangeAsync(apiArtworks);
                        break;
                    case IEnumerable<IgdbApiGame> apiGames:
                        foreach (var game in apiGames)
                        {
                            await _igdbGameModesRepository.AddOrUpdateRangeAsync(game.game_modes);
                            await _igdbMultiplayerModesRepository.AddOrUpdateRangeAsync(game.multiplayer_modes);
                            await _igdbPlatformsRepository.AddOrUpdateRangeAsync(game.platforms);
                            await _igdbThemesRepository.AddOrUpdateRangeAsync(game.themes);

                            // Join tables
                            await _igdbGames_GameModesRepository.AddOrUpdateRangeAsync(game.game_modes.Select(x => new IgdbGames_GameModes() { GameModes_SourceId = x.id, Games_SourceId = game.id }));    
                            await _igdbGames_MultiplayerModesRepository.AddOrUpdateRangeAsync(game.multiplayer_modes.Select(x => new IgdbGames_MultiplayerModes() { MultiplayerModes_SourceId = x.id, Games_SourceId = game.id }));    
                            await _igdbGames_PlatformsRepository.AddOrUpdateRangeAsync(game.platforms.Select(x => new IgdbGames_Platforms() { Platforms_SourceId = x.id, Games_SourceId = game.id }));    
                            await _igdbGames_ThemesRepository.AddOrUpdateRangeAsync(game.themes.Select(x => new IgdbGames_Themes() { Themes_SourceId = x.id, Games_SourceId = game.id }));    
                            await _igdbGames_ArtworksRepository.AddOrUpdateRangeAsync(game.artworks.Select(x => new IgdbGames_Artworks() { Artworks_SourceId = x.id, Games_SourceId = game.id }));    
                            await _igdbGames_ScreenshotsRepository.AddOrUpdateRangeAsync(game.screenshots.Select(x => new IgdbGames_Screenshots() { Screenshots_SourceId = x.id, Games_SourceId = game.id }));    
                        }

                        await _igdbGamesRepository.AddOrUpdateRangeAsync(apiGames);
                        break;
                    case IEnumerable<IgdbApiPlatform> apiPlatforms:
                        foreach (var platform in apiPlatforms)
                        {
                            await _igdbPlatformFamiliesRepository.AddOrUpdateRangeAsync(new[] { platform.platform_family });
                            await _igdbPlatformLogosRepository.AddOrUpdateRangeAsync(new[] { platform.platform_logo });

                            // Join tables
                            await _iIgdbPlatforms_PlatformFamiliesRepository.AddOrUpdateRangeAsync(new[] { new IgdbPlatforms_PlatformFamilies() { Platforms_SourceId = platform.id, PlatformFamilies_SourceId = platform.platform_family.id } });
                            await _iIgdbPlatforms_PlatformLogosRepository.AddOrUpdateRangeAsync(new[] { new IgdbPlatforms_PlatformLogos() { Platforms_SourceId = platform.id, PlatformLogos_SourceId = platform.platform_logo.id } });
                        }

                        await _igdbPlatformsRepository.AddOrUpdateRangeAsync(apiPlatforms);
                        break;
                    case IEnumerable<IgdbApiScreenshot> apiScreenshots:
                        await _igdbScreenshotsRepository.AddOrUpdateRangeAsync(apiScreenshots);
                        break;
                }
            }
        }

        private async Task<string> ExecuteRequestAsync(IgdbApiAccessToken token, string requestUri, string query)
        {
            string requestResponse = null;

            int retries = 0;
            int retryLimit = _settings.Value.RetryLimit;
        
            while (requestResponse == null)
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(_settings.Value.RequestTimeout);
                    httpClient.DefaultRequestHeaders.Add("Client-ID", _settings.Value.ClientId);
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.access_token}");
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    var content = new StringContent(query, Encoding.UTF8);
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
