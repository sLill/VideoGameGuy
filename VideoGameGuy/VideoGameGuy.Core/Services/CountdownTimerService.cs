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
            Guid sessionId = (Guid)sender;

            if (!_clientCountdownTimers.TryGetValue(sessionId, out var clientTimer))
                return;

            clientTimer.Timer.TimeRemaining -= TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS);

            if (clientTimer.Timer.TimeRemaining <= TimeSpan.Zero)
            {
                clientTimer.Timer.TimeRemaining = TimeSpan.Zero;
                await RemoveClientTimerAsync(sessionId);
            }

            // Update session data
            if (!clientTimer.Context.ConnectionAborted.IsCancellationRequested)
            {
                var sessionData = _sessionService.GetSessionData(clientTimer.Context.GetHttpContext());
                sessionData.CountdownSessionItem.TimeRemaining = clientTimer.Timer.TimeRemaining;

                _sessionService.SetSessionData(sessionData, clientTimer.Context.GetHttpContext());
                await _sessionService.CommitSessionDataAsync(clientTimer.Context.GetHttpContext());
            }

            // Message client
            await _hubContext.Clients.Client(clientTimer.Context.ConnectionId).SendAsync("UpdateTimer", clientTimer.Timer.TimeRemaining.ToString(@"mm\:ss"));
        }
        #endregion Event Handlers..

        private void AddClientTimer(Guid sessionId, HubCallerContext context, TimeSpan countdownTime)
        {
            var countdownTimer = new Timer(clientCountdownTimer_Elapsed, sessionId, TimeSpan.Zero, TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS));
            _clientCountdownTimers[sessionId] = (context, new ClientCountdownTimer() { TimeRemaining = countdownTime, Timer = countdownTimer });
        }

        private async Task RemoveClientTimerAsync(Guid sessionId)
        {
            _clientCountdownTimers.Remove(sessionId, out var clientTimer);
            await clientTimer.Timer.Timer.DisposeAsync();
            clientTimer.Timer = default;
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
