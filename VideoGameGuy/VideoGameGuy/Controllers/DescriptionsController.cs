using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using VideoGameGuy.Common;
using VideoGameGuy.Core;
using VideoGameGuy.Data;

namespace VideoGameGuy.Controllers
{
    public class DescriptionsController : Controller
    {
        #region Fields..
        private const string TITLE_EXPRESSION = @"(?:(?:\:.*)|[^a-z\u00C0-\u024F])|(?:\b[iivx]+\b)|(?:^the)";
        public const int ROUND_TIME_LIMIT_MINUTES = 2;

        private readonly ILogger<DescriptionsController> _logger;
        private readonly ISessionService _sessionService;
        private readonly ICountdownTimerService _countdownTimerService;
        private readonly IIgdbGamesRepository _igdbGamesRepository;
        private readonly ISystemStatusRepository _systemStatusRepository;
        private readonly IGameRepository _gameRepository;

        private static List<IgdbGame> _games;
        private static Regex _titleRegex; 
        
        private Random _random = new Random();
        #endregion Fields..

        #region Constructors..
        public DescriptionsController(ILogger<DescriptionsController> logger,
                                      ISessionService sessionService,
                                      ICountdownTimerService countdownTimerService,
                                      IIgdbGamesRepository igdbGamesRepository,
                                      ISystemStatusRepository systemStatusRepository,
                                      IGameRepository gameRepository)
        {
            _logger = logger;
            _sessionService = sessionService;
            _countdownTimerService = countdownTimerService;
            _igdbGamesRepository = igdbGamesRepository;
            _systemStatusRepository = systemStatusRepository;
            _gameRepository = gameRepository;
        }
        #endregion Constructors..

        #region Methods..
        #region Actions..
        public async Task<IActionResult> Index()
        {
            // Try load existing session data or create a new one
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);

            bool outOfTime = sessionData.CountdownSessionItem.TimeRemaining <= TimeSpan.Zero;

            if (outOfTime || sessionData.DescriptionsSessionItem == null|| sessionData.DescriptionsSessionItem.CurrentRound == null)
                await StartNewRoundAsync(sessionData);

            var descriptionsViewModel = await GetViewModelFromSessionDataAsync(sessionData);
            return View(descriptionsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GoNext()
        {
            await UnpauseTimer();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Restart()
        {
            // Clear cache 
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            sessionData.DescriptionsSessionItem = null;

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            await _countdownTimerService.RemoveClientTimerAsync(sessionData.SessionId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Skip()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            
            if (sessionData.DescriptionsSessionItem.CurrentRound != null)
                sessionData.DescriptionsSessionItem.CurrentRound.IsSkipped = true;

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);

            return Json(new { });
        }

        [HttpPost]
        public async Task<ActionResult> PauseTimer()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            _countdownTimerService.PauseClientTimer(sessionData.SessionId);

            return Json(new { });
        }

        [HttpPost]
        public async Task<ActionResult> UnpauseTimer()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            _countdownTimerService.UnpauseClientTimer(sessionData.SessionId);

            return Json(new { });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTimer(string timeRemaining)
        {
            // Update session data
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            sessionData.CountdownSessionItem.TimeRemaining = TimeSpan.ParseExact(timeRemaining, @"mm\:ss", CultureInfo.InvariantCulture);

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);

            return Json(new { });
        }

        public async Task<ActionResult> Validate()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);

            if (sessionData.DescriptionsSessionItem.CurrentRound != null)
            {
                sessionData.DescriptionsSessionItem.CurrentRound.IsSolved = true;
                await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            }

            return Json(new { sessionData.DescriptionsSessionItem.CurrentScore });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        #endregion Actions..

        public async Task<DescriptionsViewModel> GetViewModelFromSessionDataAsync(SessionData sessionData)
        {
            var systemStatus = await _systemStatusRepository.GetCurrentStatusAsync();

            var descriptionsViewModel = new DescriptionsViewModel()
            {
                SessionId = sessionData.SessionId,
                HighestScore = sessionData.DescriptionsSessionItem.HighestScore,
                CurrentScore = sessionData.DescriptionsSessionItem.CurrentScore,
                CurrentRound = sessionData.DescriptionsSessionItem.CurrentRound,
                TimeRemaining = sessionData.CountdownSessionItem.TimeRemaining,
                Igdb_UpdatedOnUtc = systemStatus.Igdb_UpdatedOnUtc ?? DateTime.MinValue
            };

            return descriptionsViewModel;
        }

        private async Task StartNewRoundAsync(SessionData sessionData)
        {
            if (_games == null)
            {
                _titleRegex = _titleRegex ?? new Regex(TITLE_EXPRESSION);
                _games = await _igdbGamesRepository.GetGamesWithStorylines(150);
                _games = _games.Where(x =>
                {
                    // Filter out games whose storyline contains the title
                    string formattedStoryine = _titleRegex.Replace(x.Storyline.ToLower(), string.Empty);
                    string formattedName = _titleRegex.Replace(x.Name.ToLower(), string.Empty);
                    return !formattedStoryine.Contains(formattedName);
                })?.ToList() ?? new List<IgdbGame>();
            }

            if (_games?.Any() ?? false)
            {
                IgdbGame game = _games.TakeRandom(1).FirstOrDefault();

                if (game != default)
                {
                    sessionData.DescriptionsSessionItem = sessionData.DescriptionsSessionItem ?? new DescriptionsSessionItem();
                    sessionData.DescriptionsSessionItem.DescriptionsRounds.Add(new DescriptionsSessionItem.DescriptionsRound()
                    {
                        GameTitle = game.Name,
                        GameDescription = game.Storyline,
                    });

                    await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
                }
            }
        }
        #endregion Methods..
    }
}
