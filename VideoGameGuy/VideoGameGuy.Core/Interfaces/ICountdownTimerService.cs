namespace VideoGameGuy.Core
{
    public interface ICountdownTimerService
    {
        #region Methods..
        Task StartCountdownForUser(Guid sessionId, string connectionId, int countdownSeconds);
        Task SubtractTimeForUser(Guid sessionId, string connectionId, int seconds);
        #endregion Methods..
    }
}
