using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGameGuy.Common;
using VideoGameGuy.Core;
using VideoGameGuy.Data;
using static VideoGameGuy.Data.ScreenshotsSessionItem;

namespace VideoGameGuy.Controllers
{
    public class ScreenshotsController : Controller
    {
        #region Fields..
        private readonly ILogger<ScreenshotsController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IIgdbGamesRepository _igdbGamesRepository;
        private readonly ISystemStatusRepository _systemStatusRepository;

        private static List<IgdbGame> _gamesWithArtwork;
        private static List<IgdbGame> _gamesWithScreenshots;

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
            switch (_random.Next(0, 2))
            {
                case 0:
                    await StartArtworkRoundAsync(sessionData);
                    break;
                case 1:
                    await StartScreenshotRoundAsync(sessionData);
                    break;
            }
        }

        private async Task StartArtworkRoundAsync(SessionData sessionData)
        {
            _gamesWithArtwork = _gamesWithArtwork ?? await _igdbGamesRepository.GetGamesWithArtwork(3);
            IgdbGame game = _gamesWithArtwork?.TakeRandom(1).FirstOrDefault();

            List<IgdbArtwork> artwork = await _igdbGamesRepository.GetArtworkFromGameAsync(game);
            List<ImageRecord> imageCollection = artwork.TakeRandom(3)
                .Select(x => new ImageRecord() { Value = x.Url.Replace("t_thumb", "t_screenshot_huge") }).ToList();

            if (game != default)
            {
                sessionData.ScreenshotsSessionItem.ScreenshotsRounds.Add(new ScreenshotsRound()
                {
                    GameTitle = game.Name,
                    ImageCollection = imageCollection
                });

                await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            }
        }

        private async Task StartScreenshotRoundAsync(SessionData sessionData)
        {
            _gamesWithScreenshots = _gamesWithScreenshots ?? await _igdbGamesRepository.GetGamesWithScreenshots(3);
            IgdbGame game = _gamesWithScreenshots?.TakeRandom(1).FirstOrDefault();

            List<IgdbScreenshot> screenshots = await _igdbGamesRepository.GetScreenshotsFromGameAsync(game);
            List<ImageRecord> imageCollection = screenshots.TakeRandom(3)
                .Select(x => new ImageRecord() { Value = x.Url.Replace("t_thumb", "t_screenshot_huge") }).ToList();

            if (game != default)
            {
                sessionData.ScreenshotsSessionItem.ScreenshotsRounds.Add(new ScreenshotsRound()
                {
                    GameTitle = game.Name,
                    ImageCollection = imageCollection
                });

                await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            }
        }
        #endregion Methods..
    }
}
