using Newtonsoft.Json;
using VideoGameGuy.Data;

namespace VideoGameGuy.Core
{
    public class SessionService : ISessionService
    {
        #region Fields..
        private readonly ILogger<SessionService> _logger;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public SessionService(ILogger<SessionService> logger)
        {
            _logger = logger;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<SessionData> GetSessionDataAsync(HttpContext httpContext)
        {
            SessionData sessionData = default;

            try
            {
                await LoadSessionDataAsync(httpContext);
                var sessionDataString = httpContext.Session.GetString(nameof(SessionData));

                if (!string.IsNullOrEmpty(sessionDataString))
                    sessionData = JsonConvert.DeserializeObject<SessionData>(sessionDataString);
                else
                {
                    sessionData = new SessionData();
                    SetSessionDataAsync(sessionData, httpContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return sessionData;
        }

        public async Task SetSessionDataAsync(SessionData sessionData, HttpContext httpContext)
        {
            try
            {
                httpContext.Session.SetString(nameof(SessionData), JsonConvert.SerializeObject(sessionData));
                await CommitSessionDataAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task CommitSessionDataAsync(HttpContext httpContext)
        {
            try
            {
                await httpContext.Session.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }
        }

        private async Task LoadSessionDataAsync(HttpContext httpContext)
        {
            try
            {
                await httpContext.Session.LoadAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }
        }
        #endregion Methods..
    }
}
