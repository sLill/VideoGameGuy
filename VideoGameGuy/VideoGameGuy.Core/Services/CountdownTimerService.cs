using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace VideoGameGuy.Core
{
    public class CountdownTimerService : ICountdownTimerService
    {
        #region Fields..
        private const int TIMER_INTERVAL_SECONDS = 1;

        private readonly IHubContext<CountdownTimerHub> _hubContext;
        private ConcurrentDictionary<Guid, (string ConnectionId, ClientCountdownTimer Timer)> _clientCountdownTimers = new ConcurrentDictionary<Guid, (string ConnectionId, ClientCountdownTimer Timer)>();
        #endregion Fields..

        #region Properties..
        #endregion Properties..

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
        #region Event Handlers..
        private async void clientCountdownTimer_Elapsed(object sender)
        {
            Guid sessionId = (Guid)sender;

            if (!_clientCountdownTimers.TryGetValue(sessionId, out var clientTimer))
                return;

            clientTimer.Timer.TimeRemaining -= TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS);

            if (clientTimer.Timer.TimeRemaining <= TimeSpan.Zero)
                _clientCountdownTimers.Remove(sessionId, out _);

            await _hubContext.Clients.Client(clientTimer.ConnectionId).SendAsync("UpdateTimer", clientTimer.Timer.TimeRemaining.ToString(@"mm\:ss"));
        }
        #endregion Event Handlers..

        public async Task StartCountdownForUser(Guid sessionId, string connectionId, int countdownSeconds)
        {
            // If a timer already exists for this sessionId, just update the associated connectionId
            _clientCountdownTimers.TryGetValue(sessionId, out var clientTimer);
            if (clientTimer != default)
            {
                clientTimer.ConnectionId = connectionId;
                _clientCountdownTimers[sessionId] = clientTimer;
            }
            else
                AddClientTimer(sessionId, connectionId, TimeSpan.FromSeconds(countdownSeconds));
        }

        private void AddClientTimer(Guid sessionId, string clientConnectionId, TimeSpan countdownTime)
        {
            var countdownTimer = new Timer(clientCountdownTimer_Elapsed, sessionId, TimeSpan.Zero, TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS));
            _clientCountdownTimers[sessionId] = (clientConnectionId, new ClientCountdownTimer() { TimeRemaining = countdownTime, Timer = countdownTimer });
        }

        private void RemoveClientTimer(Guid sessionId)
        {
            _clientCountdownTimers.Remove(sessionId, out _);
        }
        #endregion Methods..
    }
}
