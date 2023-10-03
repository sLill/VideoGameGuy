using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VideoGameGuy.Core
{
    public class CountdownTimerHub : Hub
    {
        #region Fields..
        private ICountdownTimerService _countdownTimerService;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Structs..
        private struct CountdownData
        {
            public Guid SessionId;
            public int CountdownSeconds;
        }
        #endregion Structs..

        #region Constructors..
        public CountdownTimerHub(ICountdownTimerService countdownTimerService)
        {
            _countdownTimerService = countdownTimerService;
        }
        #endregion Constructors..

        #region Methods..
        #region Event Handlers..
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        #endregion Event Handlers..		

        public async Task StartCountdownForUser(object data)
        {
            var countdownData = JsonConvert.DeserializeObject<CountdownData>(data.ToString());
            _countdownTimerService.StartCountdownForUser(countdownData.SessionId, Context.ConnectionId, countdownData.CountdownSeconds);
        }
        #endregion Methods..
    }
}
