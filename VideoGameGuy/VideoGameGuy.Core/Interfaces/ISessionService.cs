using VideoGameGuy.Data;

namespace VideoGameGuy.Core
{
    public interface ISessionService
    {
        #region Methods..
        Task CommitSessionDataAsync(HttpContext httpContext);
        Task LoadSessionDataAsync(HttpContext httpContext);

        SessionData GetSessionData(HttpContext httpContext);
        void SetSessionData(SessionData sessionData, HttpContext httpContext);
        #endregion Methods..
    }
}
