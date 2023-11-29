using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using VideoGameGuy.Common;
using VideoGameGuy.Core;
using VideoGameGuy.Data;
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
        private static Queue<(IgdbGame Game, List<ImageRecord> Images)> _gameQueue = new Queue<(IgdbGame Game, List<ImageRecord> Images)>();

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
            if (_gameQueue.Count < 2)
                FillGameQueue(2);

            // Try load existing session data or create a new one
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);

            bool outOfTime = sessionData.ScreenshotCountdownSessionItem.TimeRemaining <= TimeSpan.Zero;
            if (outOfTime || sessionData.ScreenshotsSessionItem == null || sessionData.ScreenshotsSessionItem.CurrentRound == null)
                await StartNewRoundAsync(sessionData);

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

            await _countdownTimerService.RemoveClientTimerAsync(sessionData.ScreenshotsSessionItem.SessionItemId);

            sessionData.ScreenshotsSessionItem = new ScreenshotsSessionItem();
            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);

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
            _countdownTimerService.PauseClientTimer(sessionData.ScreenshotsSessionItem.SessionItemId);

            return Json(new { });
        }

        [HttpPost]
        public async Task<ActionResult> UnpauseTimer()
        {
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            _countdownTimerService.UnpauseClientTimer(sessionData.ScreenshotsSessionItem.SessionItemId);

            return Json(new { });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTimer(string timeRemaining)
        {
            // Update session data
            var sessionData = await _sessionService.GetSessionDataAsync(HttpContext);
            sessionData.ScreenshotCountdownSessionItem.TimeRemaining = TimeSpan.ParseExact(timeRemaining, @"mm\:ss", CultureInfo.InvariantCulture);

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
            ScreenshotsViewModel screenshotsViewModel = null;

            try
            {
                var systemStatus = await _systemStatusRepository.GetCurrentStatusAsync();
                screenshotsViewModel = new ScreenshotsViewModel()
                {
                    SessionItemId = sessionData.ScreenshotsSessionItem.SessionItemId,
                    HighestScore = sessionData.ScreenshotsSessionItem.HighestScore,
                    CurrentScore = sessionData.ScreenshotsSessionItem.CurrentScore,
                    CurrentRound = sessionData.ScreenshotsSessionItem.CurrentRound,
                    TimeRemaining = sessionData.ScreenshotCountdownSessionItem.TimeRemaining,
                    Igdb_UpdatedOnUtc = systemStatus.Igdb_UpdatedOnUtc ?? DateTime.MinValue
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return screenshotsViewModel;
        }

        private async Task StartNewRoundAsync(SessionData sessionData)
        {
            var game = _gameQueue.Dequeue();
            Debug.WriteLine($"Writing game data for {game.Game.Name}");

            sessionData.ScreenshotsSessionItem.ScreenshotsRounds.Add(new ScreenshotsRound()
            {
                GameTitle = game.Game.Name,
                ImageCollection = game.Images
            });

            FillGameQueue(2);

            await _sessionService.SetSessionDataAsync(sessionData, HttpContext);
        }

        private void FillGameQueue(int count)
        {
            while (_gameQueue?.Count < count)
                QueueRandomRound();
        }

        private void QueueRandomRound() 
        {
            switch (_random.Next(0, 3))
            {
                case 0:
                    AddArtworkRound();
                    break;
                case 1:
                case 2:
                    AddScreenshotRound();
                    break;
            }
        }

        private void AddArtworkRound()
        {
            _gamesWithArtwork = _gamesWithArtwork ?? _igdbGamesRepository.GetGamesWithArtwork(3, 200);
            IgdbGame game = _gamesWithArtwork?.TakeRandom(1).FirstOrDefault();

            List<IgdbArtwork> artwork = _igdbGamesRepository.GetArtworkFromGame(game);
            List<ImageRecord> imageCollection = artwork.TakeRandom(3)
                .Select(x => new ImageRecord() { Value = x.Url.Replace("t_thumb", DESKTOP_IMAGE_SIZE) }).ToList();

            _gameQueue.Enqueue((game, imageCollection));
        }

        private void AddScreenshotRound()
        {
            _gamesWithScreenshots = _gamesWithScreenshots ?? _igdbGamesRepository.GetGamesWithScreenshots(3, 200);
            IgdbGame game = _gamesWithScreenshots?.TakeRandom(1).FirstOrDefault();

            List<IgdbScreenshot> screenshots = _igdbGamesRepository.GetScreenshotsFromGame(game);
            List<ImageRecord> imageCollection = screenshots.TakeRandom(3)
                .Select(x => new ImageRecord() { Value = x.Url.Replace("t_thumb", DESKTOP_IMAGE_SIZE) }).ToList();

            _gameQueue.Enqueue((game, imageCollection));
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
