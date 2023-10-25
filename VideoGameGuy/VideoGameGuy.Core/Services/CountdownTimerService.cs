using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using VideoGameGuy.Data;

namespace VideoGameGuy.Core
{
    public class CountdownTimerService : ICountdownTimerService
    {
        #region Fields..
        private const int TIMER_INTERVAL_SECONDS = 1;

        private readonly IHubContext<CountdownTimerHub> _hubContext;
        private readonly ISessionService _sessionService;

        private ConcurrentDictionary<Guid, (HubCallerContext Context, ClientCountdownTimer Timer)> _clientCountdownTimers = new ConcurrentDictionary<Guid, (HubCallerContext Context, ClientCountdownTimer Timer)>();
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Records..
        private record ClientCountdownTimer
        {
            public bool IsPaused { get; set; }
            public TimeSpan TimeRemaining { get; set; }
            public Timer Timer { get; set; }
        }
        #endregion Records..

        #region Constructors..
        public CountdownTimerService(IHubContext<CountdownTimerHub> hubContext,
                                     ISessionService sessionService)
        {
            _hubContext = hubContext;
            _sessionService = sessionService;
        }
        #endregion Constructors..

        #region Methods..
        #region Event Handlers..
        private async void clientCountdownTimer_Elapsed(object sender)
        {
            Guid clientSessionId = (Guid)sender;

            if (!_clientCountdownTimers.TryGetValue(clientSessionId, out var clientTimer))
                return;

            if (!clientTimer.Timer.IsPaused)
            {
                clientTimer.Timer.TimeRemaining -= TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS);

                if (clientTimer.Timer.TimeRemaining <= TimeSpan.Zero)
                {
                    clientTimer.Timer.TimeRemaining = TimeSpan.Zero;
                    await RemoveClientTimerAsync(clientSessionId);
                }

                // Message client
                await _hubContext.Clients.Client(clientTimer.Context.ConnectionId).SendAsync("UpdateTimer", clientTimer.Timer.TimeRemaining.ToString(@"mm\:ss"));
            }
        }
        #endregion Event Handlers..

        private void AddClientTimer(Guid sessionId, HubCallerContext context, TimeSpan countdownTime)
        {
            var countdownTimer = new Timer(clientCountdownTimer_Elapsed, sessionId, TimeSpan.Zero, TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS));
            _clientCountdownTimers[sessionId] = (context, new ClientCountdownTimer() { TimeRemaining = countdownTime, Timer = countdownTimer });
        }

        public async Task RemoveClientTimerAsync(Guid sessionId)
        {
            if (_clientCountdownTimers.ContainsKey(sessionId))
            {
                _clientCountdownTimers.Remove(sessionId, out (HubCallerContext Context, ClientCountdownTimer Timer) clientCountdownTimer);

                await clientCountdownTimer.Timer.Timer.DisposeAsync();
                clientCountdownTimer = default;
            }
        }

        public void PauseClientTimer(Guid clientSessionId)
        {
            if (!_clientCountdownTimers.TryGetValue(clientSessionId, out var clientTimer))
                return;

            clientTimer.Timer.IsPaused = true;
        }

        public void UnpauseClientTimer(Guid clientSessionId)
        {
            if (!_clientCountdownTimers.TryGetValue(clientSessionId, out var clientTimer))
                return;

            clientTimer.Timer.IsPaused = false;
        }

        public async Task StartCountdownForUser(Guid sessionId, HubCallerContext context, int countdownSeconds)
        {
            // If a timer already exists for this sessionId, just update the associated connectionId
            _clientCountdownTimers.TryGetValue(sessionId, out var clientTimer);
            if (clientTimer != default)
            {
                clientTimer.Context = context;
                _clientCountdownTimers[sessionId] = clientTimer;
            }
            else
                AddClientTimer(sessionId, context, TimeSpan.FromSeconds(countdownSeconds));
        }

        public async Task SubtractTimeForUser(Guid sessionId, HubCallerContext context, int seconds)
        {
            // If a timer already exists for this sessionId, just update the associated connectionId
            _clientCountdownTimers.TryGetValue(sessionId, out var clientTimer);
            if (clientTimer != default)
            {
                clientTimer.Context = context;
                clientTimer.Timer.TimeRemaining = clientTimer.Timer.TimeRemaining - TimeSpan.FromSeconds(seconds);
                _clientCountdownTimers[sessionId] = clientTimer;
            }
        }
        #endregion Methods..
    }
}
