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
        public async Task CommitSessionDataAsync(HttpContext httpContext)
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

        public async Task LoadSessionDataAsync(HttpContext httpContext)
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

        public SessionData GetSessionData(HttpContext httpContext)
        {
            SessionData sessionData = default;

            try
            {
                var sessionDataString = httpContext.Session.GetString(nameof(SessionData));

                if (!string.IsNullOrEmpty(sessionDataString))
                    sessionData = JsonConvert.DeserializeObject<SessionData>(sessionDataString);
                else
                {
                    sessionData = new SessionData();
                    SetSessionData(sessionData, httpContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return sessionData;
        }

        public void SetSessionData(SessionData sessionData, HttpContext httpContext) 
        {
            try
            {
                httpContext.Session.SetString(nameof(SessionData), JsonConvert.SerializeObject(sessionData));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }
}
        #endregion Methods..
    }
}
