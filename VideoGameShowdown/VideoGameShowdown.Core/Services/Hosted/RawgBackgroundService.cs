using Microsoft.Extensions.Options;
using VideoGameShowdown.Configuration;

namespace VideoGameShowdown.Core
{
    public class RawgBackgroundService : BackgroundService
    {
        #region Fields..
        private readonly ILogger<RawgBackgroundService> _logger;
        private readonly IOptions<RawgApiSettings> _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        #endregion Fields..

        #region Properties..
        private string LocalCachePath
            => Path.Combine(AppContext.BaseDirectory, _settings.Value.LocalCache_RelativePath);
        #endregion Properties..

        #region Constructors..
        public RawgBackgroundService(ILogger<RawgBackgroundService> logger,
                                     IOptions<RawgApiSettings> settings,
                                     IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }
        #endregion Constructors..

        #region Methods..
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await PollAndCacheAsync();
                await Task.Delay(TimeSpan.FromMinutes(_settings.Value.PollingInterval_Minutes), cancellationToken);
            }
        }

        private async Task PollAndCacheAsync()
        {
            _logger.LogInformation($"[RAWG] Updating data");

            await UpdateGameDataAsync();
            
            _logger.LogInformation($"[RAWG] Update complete");
        }

        private async Task UpdateGameDataAsync()
        {
            try
            {
                _logger.LogInformation($"[RAWG] Updating game data..");

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    string baseUrl = Path.Combine(_settings.Value.ApiUrl, _settings.Value.Endpoints["Games"]);
                    string requestUrl = $"{baseUrl}?key={_settings.Value.ApiKey}";
                    
                    var response = await httpClient.GetAsync(requestUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Cache the response to local file
                        Directory.CreateDirectory(LocalCachePath);
                        string filepath = Path.Combine(LocalCachePath, "games.json");
                        
                        File.WriteAllText(filepath, responseContent);
                    }
                    else
                        throw new Exception($"Api request returned status code: {response.StatusCode}");
                }

                _logger.LogInformation($"[RAWG] Update game data success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[RAWG] Update game data failed. {ex.Message} - {ex.StackTrace}");
            }
        }
        #endregion Methods..
    }
}
