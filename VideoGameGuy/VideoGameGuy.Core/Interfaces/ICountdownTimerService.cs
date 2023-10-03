namespace VideoGameGuy.Core
{
    public interface ICountdownTimerService
    {
        #region Methods..
        Task StartCountdownForUser(Guid sessionId, string connectionId, int countdownSeconds);
        #endregion Methods..
    }
}
