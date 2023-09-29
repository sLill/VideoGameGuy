using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace VideoGameGuy.Core
{
    public class CountdownTimerService : ICountdownTimerService
    {
        #region Fields..
        private const int TIMER_INTERVAL_SECONDS = 1;

        private readonly IHubContext<CountdownTimerHub> _hubContext;

        private ConcurrentDictionary<string, ClientCountdownTimer> _clientCountdownTimers = new ConcurrentDictionary<string, ClientCountdownTimer>();
        #endregion Fields..

        #region Records..
        private record ClientCountdownTimer
        {
            public TimeSpan TimeRemaining { get; set; }
            public Timer Timer { get; set; }
        }
        #endregion Records..

        #region Constructors..
        public CountdownTimerService(IHubContext<CountdownTimerHub> hubContext)
        {
            _hubContext = hubContext;
        }
        #endregion Constructors..

        #region Methods..
        #region EventHandlers..
        private async void clientCountdownTimer_Elapsed(object sender)
        {
            var clientConnectionId = sender as string;

            if (!_clientCountdownTimers.TryGetValue(clientConnectionId, out var clientCountdownTimer)) 
                return;

            clientCountdownTimer.TimeRemaining -= TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS);

            if (clientCountdownTimer.TimeRemaining <= TimeSpan.Zero)
                _clientCountdownTimers.Remove(clientConnectionId, out _);

            await _hubContext.Clients.Client(clientConnectionId).SendAsync("TimeReceived", clientCountdownTimer.TimeRemaining.ToString(@"mm\:ss"));
        }
        #endregion EventHandlers..

        public void AddClient(string clientConnectionId, TimeSpan countdownTime)
        {
            var countdownTimer = new Timer(clientCountdownTimer_Elapsed, clientConnectionId, TimeSpan.Zero, TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS));
            _clientCountdownTimers[clientConnectionId] = new ClientCountdownTimer() { TimeRemaining = countdownTime, Timer = countdownTimer };  
        }

        public void RemoveClient(string clientConnectionId)
        {
            _clientCountdownTimers.Remove(clientConnectionId, out _);
        }
        #endregion Methods..
    }
}
