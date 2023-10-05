using VideoGameGuy.Data;

namespace VideoGameGuy.Core
{
    public interface ISessionService
    {
        #region Methods..
        Task<SessionData> GetSessionDataAsync(HttpContext httpContext);
        Task SetSessionDataAsync(SessionData sessionData, HttpContext httpContext);
        #endregion Methods..
    }
}
