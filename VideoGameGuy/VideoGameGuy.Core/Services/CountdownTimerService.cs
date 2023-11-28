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
            Guid sessionItemId = (Guid)sender;

            if (!_clientCountdownTimers.TryGetValue(sessionItemId, out var clientTimer))
                return;

            if (!clientTimer.Timer.IsPaused)
            {
                clientTimer.Timer.TimeRemaining -= TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS);

                if (clientTimer.Timer.TimeRemaining <= TimeSpan.Zero)
                {
                    clientTimer.Timer.TimeRemaining = TimeSpan.Zero;
                    await RemoveClientTimerAsync(sessionItemId);
                }

                // Message client
                await _hubContext.Clients.Client(clientTimer.Context.ConnectionId).SendAsync("UpdateTimer", clientTimer.Timer.TimeRemaining.ToString(@"mm\:ss"));
            }
        }
        #endregion Event Handlers..

        private void AddClientTimer(Guid sessionItemId, HubCallerContext context, TimeSpan countdownTime)
        {
            var countdownTimer = new Timer(clientCountdownTimer_Elapsed, sessionItemId, TimeSpan.Zero, TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS));
            _clientCountdownTimers[sessionItemId] = (context, new ClientCountdownTimer() { TimeRemaining = countdownTime, Timer = countdownTimer });
        }

        public async Task RemoveClientTimerAsync(Guid sessionItemId)
        {
            if (_clientCountdownTimers.ContainsKey(sessionItemId))
            {
                _clientCountdownTimers.Remove(sessionItemId, out (HubCallerContext Context, ClientCountdownTimer Timer) clientCountdownTimer);

                await clientCountdownTimer.Timer.Timer.DisposeAsync();
                clientCountdownTimer = default;
            }
        }

        public void PauseClientTimer(Guid sessionItemId)
        {
            if (!_clientCountdownTimers.TryGetValue(sessionItemId, out var clientTimer))
                return;

            clientTimer.Timer.IsPaused = true;
        }

        public void UnpauseClientTimer(Guid sessionItemId)
        {
            if (!_clientCountdownTimers.TryGetValue(sessionItemId, out var clientTimer))
                return;

            clientTimer.Timer.IsPaused = false;
        }

        public async Task StartCountdownForUser(Guid sessionItemId, HubCallerContext context, int countdownSeconds)
        {
            // If a timer already exists for this sessionId, just update the associated connectionId
            _clientCountdownTimers.TryGetValue(sessionItemId, out var clientTimer);
            if (clientTimer != default)
            {
                clientTimer.Context = context;
                _clientCountdownTimers[sessionItemId] = clientTimer;
            }
            else
                AddClientTimer(sessionItemId, context, TimeSpan.FromSeconds(countdownSeconds));
        }

        public async Task SubtractTimeForUser(Guid sessionItemId, HubCallerContext context, int seconds)
        {
            // If a timer already exists for this sessionId, just update the associated connectionId
            _clientCountdownTimers.TryGetValue(sessionItemId, out var clientTimer);
            if (clientTimer != default)
            {
                clientTimer.Context = context;
                clientTimer.Timer.TimeRemaining = clientTimer.Timer.TimeRemaining - TimeSpan.FromSeconds(seconds);
                _clientCountdownTimers[sessionItemId] = clientTimer;
            }
        }
        #endregion Methods..
    }
}
