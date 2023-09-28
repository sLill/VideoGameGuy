using Microsoft.AspNetCore.SignalR;

namespace VideoGameGuy.Core
{
    public class CountdownTimerHub : Hub
    {
        #region Fields..
        private readonly CountdownTimerService _countdownTimerService;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public CountdownTimerHub(CountdownTimerService countdownTimerService)
        {
            _countdownTimerService = countdownTimerService;
        }
        #endregion Constructors..

        #region Methods..
        #region Event Handlers..
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            _countdownTimerService.AddClient(Context.ConnectionId, TimeSpan.FromSeconds(1));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (!string.IsNullOrEmpty(Context.ConnectionId))
                _countdownTimerService.RemoveClient(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
        #endregion Event Handlers..		

        public void DoThing()
        {

        }
        #endregion Methods..
    }
}
