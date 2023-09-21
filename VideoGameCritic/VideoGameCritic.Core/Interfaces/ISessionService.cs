using VideoGameGuy.Data;

namespace VideoGameGuy.Core
{
    public interface ISessionService
    {
        #region Methods..
        T GetSessionData<T>(HttpContext httpContext) where T : SessionDataBase;
        void SetSessionData<T>(T sessionDataBase, HttpContext httpContext) where T : SessionDataBase;
        #endregion Methods..
    }
}
