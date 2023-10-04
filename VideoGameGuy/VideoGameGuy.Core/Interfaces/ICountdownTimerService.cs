using Microsoft.AspNetCore.SignalR;

namespace VideoGameGuy.Core
{
    public interface ICountdownTimerService
    {
        #region Methods..
        Task StartCountdownForUser(Guid sessionId, HubCallerContext context, int countdownSeconds);
        Task SubtractTimeForUser(Guid sessionId, HubCallerContext context, int seconds);
        #endregion Methods..
    }
}
