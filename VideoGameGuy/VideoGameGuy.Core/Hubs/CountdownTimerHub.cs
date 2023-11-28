using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

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
            public int Seconds;
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
            await _countdownTimerService.StartCountdownForUser(countdownData.SessionId, Context, countdownData.Seconds);
        }

        public async Task RemoveCountdownForUser(object data)
        {
            var countdownData = JsonConvert.DeserializeObject<CountdownData>(data.ToString());
            await _countdownTimerService.RemoveClientTimerAsync(countdownData.SessionId);
        }

        public async Task SubtractTimeForUser(object data)
        {
            var countdownData = JsonConvert.DeserializeObject<CountdownData>(data.ToString());
            await _countdownTimerService.SubtractTimeForUser(countdownData.SessionId, Context, countdownData.Seconds);
        }
        #endregion Methods..
    }
}
