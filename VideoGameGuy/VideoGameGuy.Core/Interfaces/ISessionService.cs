using VideoGameGuy.Data;

namespace VideoGameGuy.Core
{
    public interface ISessionService
    {
        #region Methods..
        SessionData GetSessionData(HttpContext httpContext);
        void UpdateSessionData(SessionData sessionData, HttpContext httpContext);
        #endregion Methods..
    }
}
