using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGameGuy.Common;
using VideoGameGuy.Core;
using VideoGameGuy.Data;

namespace VideoGameGuy.Controllers
{
    public class ScreenshotsController : Controller
    {
        #region Fields..
        private readonly ILogger<ScreenshotsController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IIgdbGamesRepository _igdbGamesRepository;
        private readonly ISystemStatusRepository _systemStatusRepository;

        private static List<IgdbGame> _games;
        private Random _random = new Random();
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public ScreenshotsController(ILogger<ScreenshotsController> logger,
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
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            if (sessionData.ScreenshotsSessionItem.CurrentRound == null)
                await StartNewRoundAsync(sessionData);

            var screenshotsViewModel = await GetViewModelFromSessionDataAsync(sessionData);
            return View(screenshotsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GoNext()
        {
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        #endregion Actions..

        public async Task<ScreenshotsViewModel> GetViewModelFromSessionDataAsync(SessionData sessionData)
        {
            var systemStatus = await _systemStatusRepository.GetCurrentStatusAsync();

            var screenshotsViewModel = new ScreenshotsViewModel()
            {
                SessionId = sessionData.SessionId,
                HighestScore = sessionData.ScreenshotsSessionItem.HighestScore,
                CurrentScore = sessionData.ScreenshotsSessionItem.CurrentScore,
                CurrentRound = sessionData.ScreenshotsSessionItem.CurrentRound,
                Igdb_UpdatedOnUtc = systemStatus.Igdb_UpdatedOnUtc ?? DateTime.MinValue
            };

            return screenshotsViewModel;
        }

        private async Task StartNewRoundAsync(SessionData sessionData)
        {
            _games = _games ?? await _igdbGamesRepository.GetGamesWithStorylines(200);
            IgdbGame game = _games?.TakeRandom(1).FirstOrDefault();

            if (game != default)
            {
                sessionData.ScreenshotsSessionItem.ScreenshotsRounds.Add(new ScreenshotsSessionItem.ScreenshotsRound()
                {
                    GameTitle = game.Name
                });

                await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            }
        }
        #endregion Methods..
    }
}
