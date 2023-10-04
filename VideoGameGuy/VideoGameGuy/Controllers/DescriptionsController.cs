using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using VideoGameGuy.Common;
using VideoGameGuy.Core;
using VideoGameGuy.Data;

namespace VideoGameGuy.Controllers
{
    public class DescriptionsController : Controller
    {
        #region Fields..
        public const int ROUND_TIME_LIMIT_MINUTES = 2;

        private readonly ILogger<DescriptionsController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IIgdbGamesRepository _igdbGamesRepository;
        private readonly ISystemStatusRepository _systemStatusRepository;

        private static List<IgdbGame> _games;
        private Random _random = new Random();
        #endregion Fields..

        #region Constructors..
        public DescriptionsController(ILogger<DescriptionsController> logger,
                                      ISessionService sessionService,
                                      IIgdbGamesRepository igdbGamesRepository,
                                      ISystemStatusRepository systemStatusRepository)
        {
            _logger = logger;
            _sessionService = sessionService;
            _igdbGamesRepository = igdbGamesRepository;
            _systemStatusRepository = systemStatusRepository;
        }
        #endregion Constructors..

        #region Methods..
        #region Actions..
        public async Task<IActionResult> Index()
        {
            // Try load existing session data or create a new one
            await _sessionService.LoadSessionDataAsync(HttpContext);
            var sessionData = _sessionService.GetSessionData(HttpContext);

            bool outOfTime = sessionData.CountdownSessionItem.TimeRemaining <= TimeSpan.Zero;

            if (outOfTime || sessionData.DescriptionsSessionItem.CurrentRound == null)
                await StartNewRoundAsync(sessionData);

            var descriptionsViewModel = await GetViewModelFromSessionDataAsync(sessionData);
            return View(descriptionsViewModel);
        }

        [HttpPost]
        public IActionResult GoNext()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Skip()
        {
            var sessionData = _sessionService.GetSessionData(HttpContext);
            
            if (sessionData.DescriptionsSessionItem.CurrentRound != null)
                sessionData.DescriptionsSessionItem.CurrentRound.IsSkipped = true;
            
            await StartNewRoundAsync(sessionData);

            var descriptionsViewModel = await GetViewModelFromSessionDataAsync(sessionData);
            return RedirectToAction("Index", descriptionsViewModel);
        }

        public async Task<ActionResult> Validate()
        {
            var sessionData = _sessionService.GetSessionData(HttpContext);

            if (sessionData.DescriptionsSessionItem.CurrentRound != null)
            {
                sessionData.DescriptionsSessionItem.CurrentRound.IsSolved = true;
                _sessionService.SetSessionData(sessionData, HttpContext);
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
            _games = _games ?? await _igdbGamesRepository.GetGamesWithStorylines(200);
            IgdbGame game = _games?.TakeRandom(1).FirstOrDefault();

            if (game != default)
            {
                sessionData.DescriptionsSessionItem.DescriptionsRounds.Add(new DescriptionsSessionItem.DescriptionsRound()
                {
                    GameTitle = game.Name,
                    GameDescription = game.Storyline,
                });

                _sessionService.SetSessionData(sessionData, HttpContext);
            }
        }
        #endregion Methods..
    }
}
