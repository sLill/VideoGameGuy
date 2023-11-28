using Microsoft.AspNetCore.SignalR;

namespace VideoGameGuy.Core
{
    public interface ICountdownTimerService
    {
        #region Methods..
        void PauseClientTimer(Guid sessionItemId);
        void UnpauseClientTimer(Guid sessionItemId);
        Task RemoveClientTimerAsync(Guid sessionItemId);
        Task StartCountdownForUser(Guid sessionItemId, HubCallerContext context, int countdownSeconds);
        Task SubtractTimeForUser(Guid sessionItemId, HubCallerContext context, int seconds);
        #endregion Methods..
    }
}
