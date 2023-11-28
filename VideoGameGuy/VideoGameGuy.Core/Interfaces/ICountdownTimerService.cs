using Microsoft.AspNetCore.SignalR;

namespace VideoGameGuy.Core
{
    public interface ICountdownTimerService
    {
        #region Methods..
        void PauseClientTimer(Guid clientSessionId);
        void UnpauseClientTimer(Guid clientSessionId);
        Task RemoveClientTimerAsync(Guid clientSessionId);
        Task StartCountdownForUser(Guid clientSessionId, HubCallerContext context, int countdownSeconds);
        Task SubtractTimeForUser(Guid clientSessionId, HubCallerContext context, int seconds);
        #endregion Methods..
    }
}
