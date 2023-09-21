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
        public T GetSessionData<T>(HttpContext httpContext) where T : SessionDataBase
        {
            T sessionData = null;

            try
            {
                var sessionDataString = httpContext.Session.GetString(nameof(T));
                if (!string.IsNullOrEmpty(sessionDataString))
                    sessionData = JsonConvert.DeserializeObject<T>(sessionDataString);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return sessionData;
        }

        public void SetSessionData<T>(T sessionDataBase, HttpContext httpContext) where T : SessionDataBase
        {
            try
            {
                httpContext.Session.SetString(nameof(T), JsonConvert.SerializeObject(sessionDataBase));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }
}
        #endregion Methods..
    }
}
