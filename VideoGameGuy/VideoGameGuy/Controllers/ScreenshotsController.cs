using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using VideoGameGuy.Common;
using VideoGameGuy.Core;
using VideoGameGuy.Data;
using static Azure.Core.HttpHeader;
using static VideoGameGuy.Data.ScreenshotsSessionItem;

namespace VideoGameGuy.Controllers
{
    public class ScreenshotsController : Controller
    {
        #region Fields..
        private const string DESKTOP_IMAGE_SIZE = "t_720p";

        private readonly ILogger<ScreenshotsController> _logger;
        private readonly ISessionService _sessionService;
        private readonly ICountdownTimerService _countdownTimerService;
        private readonly IIgdbGamesRepository _igdbGamesRepository;
        private readonly ISystemStatusRepository _systemStatusRepository;
        private readonly IGameRepository _gameRepository;

        private static List<IgdbGame> _gamesWithArtwork;
        private static List<IgdbGame> _gamesWithScreenshots;

        private Random _random = new Random();
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public ScreenshotsController(ILogger<ScreenshotsController> logger,
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
            if (outOfTime || sessionData.ScreenshotsSessionItem == null || sessionData.ScreenshotsSessionItem.CurrentRound == null)
                await StartNewGameAsync(sessionData);

            var screenshotsViewModel = await GetViewModelFromSessionDataAsync(sessionData);
            return View(screenshotsViewModel);
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
            sessionData.ScreenshotsSessionItem = new ScreenshotsSessionItem();

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            await _countdownTimerService.RemoveClientTimerAsync(sessionData.SessionId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Skip()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);

            if (sessionData.ScreenshotsSessionItem.CurrentRound != null)
                sessionData.ScreenshotsSessionItem.CurrentRound.IsSkipped = true;

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

        [HttpPost]
        public async Task<ActionResult> GameOver(string score)
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);

            await _gameRepository.AddAsync(new Game()
            {
                ClientIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                GameType = GameType.Screenshots,
                SessionId = sessionData.SessionId,
                GameScore = sessionData.ScreenshotsSessionItem.CurrentScore.ToString()
            });

            return Json(new { });
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
                TimeRemaining = sessionData.CountdownSessionItem.TimeRemaining,
                Igdb_UpdatedOnUtc = systemStatus.Igdb_UpdatedOnUtc ?? DateTime.MinValue
            };

            return screenshotsViewModel;
        }

        private async Task StartNewGameAsync(SessionData sessionData)
        {
            sessionData.ScreenshotsSessionItem.ScreenshotsRounds = new List<ScreenshotsRound>();

            switch (_random.Next(0, 2))
            {
                case 0:
                    await AddArtworkRoundAsync(sessionData);
                    break;
                case 1:
                    await AddScreenshotRoundAsync(sessionData);
                    break;
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

            if (sessionData.ScreenshotsSessionItem.CurrentRound != null)
            {
                sessionData.ScreenshotsSessionItem.CurrentRound.IsSolved = true;
                await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
            }

            return Json(new { sessionData.ScreenshotsSessionItem.CurrentScore });
        }
        #endregion Methods..
    }
}
