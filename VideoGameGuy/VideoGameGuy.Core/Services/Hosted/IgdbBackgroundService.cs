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

        private IgdbDbContext _idgbDbContext;

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

            _idgbDbContext = serviceScope.ServiceProvider.GetRequiredService<IgdbDbContext>();

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
            await UpdateEndpointDataAsync<IgdbApiGame>(token, IgdbApiEndpoints.Games,  updatedOnStartDate, "total_rating_count > 100", IgdbApiEndpoints.GameModes, 
                IgdbApiEndpoints.MultiplayerModes, IgdbApiEndpoints.Platforms, IgdbApiEndpoints.Themes, IgdbApiEndpoints.Artworks, IgdbApiEndpoints.Screenshots);

            await UpdateEndpointDataAsync<IgdbApiPlatformFamily>(token, IgdbApiEndpoints.PlatformFamilies, null, null);
            await UpdateEndpointDataAsync<IgdbApiPlatformLogo>(token, IgdbApiEndpoints.PlatformLogos, null, null);
            await UpdateEndpointDataAsync<IgdbApiPlatform>(token, IgdbApiEndpoints.Platforms, updatedOnStartDate, null);
        }

        private async Task UpdateEndpointDataAsync<T>(IgdbApiAccessToken token, string endpoint, DateTime? updatedOnStartDate, string? whereClause, params string[] subEndpoints) where T : class 
        {
            Type apiObjectType = typeof(T);

            try
            {
                _logger.LogInformation($"[IGDB] Updating {apiObjectType} data..");

                Stopwatch requestRateTimer = Stopwatch.StartNew();

                int offset = 0;
                int resultsPerRequest = 500;

                bool isFirstLoad = updatedOnStartDate == DateTime.MinValue;
                long unixStartTime = (updatedOnStartDate != null && updatedOnStartDate != DateTime.MinValue) ? ((DateTimeOffset)updatedOnStartDate.Value).ToUnixTimeSeconds() : 0;

                // Loop until no more data is returned
                while (true)
                {
                    string requestUri = $"{_settings.Value.ApiUrl}/{endpoint}";

                    // Build query
                    StringBuilder query = new StringBuilder($"fields *");

                    if (subEndpoints?.Any() ?? false)
                        query.Append("," + string.Join(',', subEndpoints.Select(x => $"{x}.*")));

                    query.Append(';');

                    // WHERE clause
                    var whereConditions = new List<string>();

                    if (whereClause != null)
                        whereConditions.Add(whereClause);

                    if (updatedOnStartDate != null)
                        whereConditions.Add($"updated_at >= {unixStartTime}");

                    if (whereConditions.Any())
                        query.Append($"where {string.Join(" & ", whereConditions)};");

                    query.Append($"limit {resultsPerRequest};");
                     
                    // Limit the number of requests per second
                    if (requestRateTimer.ElapsedMilliseconds >= 1000)
                    {
                        // Wait out the rest of the second before allowing requests to continue
                        int timeRemaining = 1000 - (int)requestRateTimer.ElapsedMilliseconds;
                        if (timeRemaining > 0)
                            await Task.Delay(timeRemaining);

                        requestRateTimer.Restart();
                    }

                    var responses = await Task.WhenAll(ExecuteRequestAsync(token, requestUri, query.ToString() + $"offset {offset};".ToString()),
                                                       ExecuteRequestAsync(token, requestUri, query.ToString() + $"offset {offset + resultsPerRequest};".ToString()));

                    offset += (resultsPerRequest * 2);

                    if (responses.Any())
                    {
                        List<T> apiObjectCollection = new List<T>();
                        responses.ForEach(x =>
                        {
                            if (x != null)
                            {
                                var apiObjects = ParseJsonArray<T>(x);
                                if (apiObjects?.Any() ?? false)
                                    apiObjectCollection.AddRange(apiObjects);
                            }
                        });

                        if (apiObjectCollection?.Any() ?? false)
                            await SaveEndpointDataAsync(apiObjectCollection, isFirstLoad);

                        // End of data
                        if (apiObjectCollection.Count != (resultsPerRequest * 2))
                            break;
                    }
                }

                _logger.LogInformation($"[IGDB] Update {apiObjectType} data complete");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IGDB] Update {apiObjectType} data failed. {ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task SaveEndpointDataAsync<U>(IEnumerable<U> modelObjects, bool isFirstLoad) where U : class
        {
            if (modelObjects?.Any() ?? false)
            {
                // The first time loading data from the api into a fresh db can be strenuous. We can help take some 
                // pressure off of the system by skipping checking for updated entities
                if (isFirstLoad)
                {
                    switch (modelObjects)
                    {
                        case IEnumerable<IgdbApiGame> apiGames:
                            foreach (var game in apiGames)
                            {
                                await _igdbGameModesRepository.AddRangeAsync(game.game_modes, true);
                                await _igdbMultiplayerModesRepository.AddRangeAsync(game.multiplayer_modes, true);
                                await _igdbPlatformsRepository.AddRangeAsync(game.platforms, true);
                                await _igdbThemesRepository.AddRangeAsync(game.themes, true);
                                await _igdbArtworksRepository.AddRangeAsync(game.artworks, true);
                                await _igdbScreenshotsRepository.AddRangeAsync(game.screenshots, true);

                                // Join tables
                                if (game.game_modes != null)
                                    await _igdbGames_GameModesRepository.AddRangeAsync(game.game_modes.Select(x => new IgdbGames_GameModes() { GameModes_SourceId = x.id, Games_SourceId = game.id }), true);

                                if (game.multiplayer_modes != null)
                                    await _igdbGames_MultiplayerModesRepository.AddRangeAsync(game.multiplayer_modes.Select(x => new IgdbGames_MultiplayerModes() { MultiplayerModes_SourceId = x.id, Games_SourceId = game.id }), true);

                                if (game.platforms != null)
                                    await _igdbGames_PlatformsRepository.AddRangeAsync(game.platforms.Select(x => new IgdbGames_Platforms() { Platforms_SourceId = x.id, Games_SourceId = game.id }), true);

                                if (game.themes != null)
                                    await _igdbGames_ThemesRepository.AddRangeAsync(game.themes.Select(x => new IgdbGames_Themes() { Themes_SourceId = x.id, Games_SourceId = game.id }), true);

                                if (game.artworks != null)
                                    await _igdbGames_ArtworksRepository.AddRangeAsync(game.artworks.Select(x => new IgdbGames_Artworks() { Artworks_SourceId = x.id, Games_SourceId = game.id }), true);

                                if (game.screenshots != null)
                                    await _igdbGames_ScreenshotsRepository.AddRangeAsync(game.screenshots.Select(x => new IgdbGames_Screenshots() { Screenshots_SourceId = x.id, Games_SourceId = game.id }), true);
                            }

                            await _igdbGamesRepository.AddRangeAsync(apiGames, true);
                            await _idgbDbContext.BulkSaveChangesAsync();
                            break;
                        case IEnumerable<IgdbApiPlatform> apiPlatforms:
                            await _igdbPlatformsRepository.AddRangeAsync(apiPlatforms);
                            break;
                        case IEnumerable<IgdbApiPlatformFamily> apiPlatformFamilies:
                            await _igdbPlatformFamiliesRepository.AddRangeAsync(apiPlatformFamilies);
                            break;
                        case IEnumerable<IgdbApiPlatformLogo> apiPlatformLogos:
                            await _igdbPlatformLogosRepository.AddRangeAsync(apiPlatformLogos);
                            break;
                    }
                }

                switch (modelObjects)
                {
                    case IEnumerable<IgdbApiGame> apiGames:
                        foreach (var game in apiGames)
                        {
                            await _igdbGameModesRepository.AddOrUpdateRangeAsync(game.game_modes, true);
                            await _igdbMultiplayerModesRepository.AddOrUpdateRangeAsync(game.multiplayer_modes, true);
                            await _igdbPlatformsRepository.AddOrUpdateRangeAsync(game.platforms, true);
                            await _igdbThemesRepository.AddOrUpdateRangeAsync(game.themes, true);
                            await _igdbArtworksRepository.AddOrUpdateRangeAsync(game.artworks, true);
                            await _igdbScreenshotsRepository.AddOrUpdateRangeAsync(game.screenshots, true);

                            // Join tables
                            if (game.game_modes != null)
                                await _igdbGames_GameModesRepository.AddOrUpdateRangeAsync(game.game_modes.Select(x => new IgdbGames_GameModes() { GameModes_SourceId = x.id, Games_SourceId = game.id }), true);

                            if (game.multiplayer_modes != null)
                                await _igdbGames_MultiplayerModesRepository.AddOrUpdateRangeAsync(game.multiplayer_modes.Select(x => new IgdbGames_MultiplayerModes() { MultiplayerModes_SourceId = x.id, Games_SourceId = game.id }), true);

                            if (game.platforms != null)
                                await _igdbGames_PlatformsRepository.AddOrUpdateRangeAsync(game.platforms.Select(x => new IgdbGames_Platforms() { Platforms_SourceId = x.id, Games_SourceId = game.id }), true);

                            if (game.themes != null)
                                await _igdbGames_ThemesRepository.AddOrUpdateRangeAsync(game.themes.Select(x => new IgdbGames_Themes() { Themes_SourceId = x.id, Games_SourceId = game.id }), true);

                            if (game.artworks != null)
                                await _igdbGames_ArtworksRepository.AddOrUpdateRangeAsync(game.artworks.Select(x => new IgdbGames_Artworks() { Artworks_SourceId = x.id, Games_SourceId = game.id }), true);

                            if (game.screenshots != null)
                                await _igdbGames_ScreenshotsRepository.AddOrUpdateRangeAsync(game.screenshots.Select(x => new IgdbGames_Screenshots() { Screenshots_SourceId = x.id, Games_SourceId = game.id }), true);
                        }

                        await _igdbGamesRepository.AddOrUpdateRangeAsync(apiGames, true);
                        await _idgbDbContext.BulkSaveChangesAsync();
                        break;
                    case IEnumerable<IgdbApiPlatform> apiPlatforms:
                        await _igdbPlatformsRepository.AddOrUpdateRangeAsync(apiPlatforms);
                        break;
                    case IEnumerable<IgdbApiPlatformFamily> apiPlatformFamilies:
                        await _igdbPlatformFamiliesRepository.AddOrUpdateRangeAsync(apiPlatformFamilies);
                        break;
                    case IEnumerable<IgdbApiPlatformLogo> apiPlatformLogos:
                        await _igdbPlatformLogosRepository.AddOrUpdateRangeAsync(apiPlatformLogos);
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
