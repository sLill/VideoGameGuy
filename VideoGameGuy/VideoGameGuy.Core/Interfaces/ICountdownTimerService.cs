namespace VideoGameGuy.Core
{
    public interface ICountdownTimerService
    {
        #region Methods..
        void AddClient(string clientConnectionId, TimeSpan countdownTime);
        void RemoveClient(string clientConnectionId);
        #endregion Methods..
    }
}
