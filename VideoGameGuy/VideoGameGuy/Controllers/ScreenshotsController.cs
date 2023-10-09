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
        private const int ROUND_COUNT = 15;
        private const string DESKTOP_IMAGE_SIZE = "t_720p";

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
            if (sessionData.ScreenshotsSessionItem.ScreenshotsRounds == null)
                await StartNewGameAsync(sessionData);

            var screenshotsViewModel = await GetViewModelFromSessionDataAsync(sessionData);
            return View(screenshotsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GoNext()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            int roundIndex = sessionData.ScreenshotsSessionItem.SelectedRoundIndex;
            
            if (roundIndex < ROUND_COUNT)
                sessionData.ScreenshotsSessionItem.SelectedRoundIndex = roundIndex + 1;

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> GoToRound(int roundIndex)
        {
            try
            {
                var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
                sessionData.ScreenshotsSessionItem.SelectedRoundIndex = roundIndex;
                await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not navigate user to round. RoundIndex: {roundIndex}. {ex.Message} - {ex.StackTrace}");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Skip()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);

            int roundIndex = sessionData.ScreenshotsSessionItem.SelectedRoundIndex;
            if (roundIndex < ROUND_COUNT)
                sessionData.ScreenshotsSessionItem.SelectedRoundIndex = roundIndex + 1;

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);

            return Json(new { });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        #endregion Actions..

        public async Task<ScreenshotsViewModel> GetViewModelFromSessionDataAsync(SessionData sessionData)
        {
            var systemStatus = await _systemStatusRepository.GetCurrentStatusAsync();

            int roundIndex = sessionData.ScreenshotsSessionItem.SelectedRoundIndex;
            var screenshotsViewModel = new ScreenshotsViewModel()
            {
                SessionId = sessionData.SessionId,
                HighestScore = sessionData.ScreenshotsSessionItem.HighestScore,
                CurrentScore = sessionData.ScreenshotsSessionItem.CurrentScore,
                ScreenshotsRounds = sessionData.ScreenshotsSessionItem.ScreenshotsRounds,
                SelectedRound =  sessionData.ScreenshotsSessionItem.ScreenshotsRounds[roundIndex],
                Igdb_UpdatedOnUtc = systemStatus.Igdb_UpdatedOnUtc ?? DateTime.MinValue
            };

            return screenshotsViewModel;
        }

        private async Task StartNewGameAsync(SessionData sessionData)
        {
            sessionData.ScreenshotsSessionItem.ScreenshotsRounds = new List<ScreenshotsRound>();

            for (int i = 0; i < ROUND_COUNT; i++)
            {
                switch (_random.Next(0, 2))
                {
                    case 0:
                        await AddArtworkRoundAsync(sessionData);
                        break;
                    case 1:
                        await AddScreenshotRoundAsync(sessionData);
                        break;
                }
            }

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
        }

        private async Task AddArtworkRoundAsync(SessionData sessionData)
        {
            _gamesWithArtwork = _gamesWithArtwork ?? await _igdbGamesRepository.GetGamesWithArtwork(3);
            IgdbGame game = _gamesWithArtwork?.TakeRandom(1).FirstOrDefault();

            List<IgdbArtwork> artwork = await _igdbGamesRepository.GetArtworkFromGameAsync(game);
            List<ImageRecord> imageCollection = artwork.TakeRandom(5)
                .Select(x => new ImageRecord() { Value = x.Url.Replace("t_thumb", DESKTOP_IMAGE_SIZE) }).ToList();

            if (game != default)
            {
                sessionData.ScreenshotsSessionItem.ScreenshotsRounds.Add(new ScreenshotsRound()
                {
                    GameTitle = game.Name,
                    ImageCollection = imageCollection
                });
            }
        }

        private async Task AddScreenshotRoundAsync(SessionData sessionData)
        {
            _gamesWithScreenshots = _gamesWithScreenshots ?? await _igdbGamesRepository.GetGamesWithScreenshots(3);
            IgdbGame game = _gamesWithScreenshots?.TakeRandom(1).FirstOrDefault();

            List<IgdbScreenshot> screenshots = await _igdbGamesRepository.GetScreenshotsFromGameAsync(game);
            List<ImageRecord> imageCollection = screenshots.TakeRandom(5)
                .Select(x => new ImageRecord() { Value = x.Url.Replace("t_thumb", DESKTOP_IMAGE_SIZE) }).ToList();

            if (game != default)
            {
                sessionData.ScreenshotsSessionItem.ScreenshotsRounds.Add(new ScreenshotsRound()
                {
                    GameTitle = game.Name,
                    ImageCollection = imageCollection
                });
            }
        }

        public async Task<ActionResult> Validate()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);

            int roundIndex = sessionData.ScreenshotsSessionItem.SelectedRoundIndex;
            sessionData.ScreenshotsSessionItem.ScreenshotsRounds[roundIndex].IsSolved = true;

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            return Json(new { sessionData.ScreenshotsSessionItem.CurrentScore });
        }
        #endregion Methods..
    }
}
