using Microsoft.AspNetCore.Mvc;
using VideoGameGuy.Core;
using VideoGameGuy.Data;

namespace VideoGameGuy.Controllers
{
    public class ReviewScoresController : Controller
    {
        #region Fields..
        private readonly ILogger<ReviewScoresController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISessionService _sessionService;
        private readonly IRawgGamesRepository _rawgGamesRepository;
        private readonly ISystemStatusRepository _systemStatusRepository;
        #endregion Fields..

        #region Constructors..
        public ReviewScoresController(ILogger<ReviewScoresController> logger,
                                      IWebHostEnvironment webHostEnvironment,
                                      ISessionService sessionService,
                                      IRawgGamesRepository rawgGamesRepository,
                                      ISystemStatusRepository systemStatusRepository)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _sessionService = sessionService;
            _rawgGamesRepository = rawgGamesRepository;
            _systemStatusRepository = systemStatusRepository;
        }
        #endregion Constructors..

        #region Methods..
        #region Actions..
        [HttpPost]
        public async Task<ActionResult> UserVote(Guid userChoiceId)
        {
            object? result = null;

            // Ignore vote if user has already voted for this set of games before
            var sessionData = _sessionService.GetSessionData(HttpContext);
            
            if (sessionData.ReviewScoresSessionItem.CurrentRound != null)
            {
                //var reviewScoresViewModel = await GetViewModelFromSessionDataAsync(sessionData);
                RawgGame gameOne = await _rawgGamesRepository.GetGameFromGameIdAsync(sessionData.ReviewScoresSessionItem.CurrentRound.GameOneId);
                RawgGame gameTwo = await _rawgGamesRepository.GetGameFromGameIdAsync(sessionData.ReviewScoresSessionItem.CurrentRound.GameTwoId);

                Guid? winningGameId = await GetWinningGameIdAsync(gameOne, gameTwo);
                sessionData.ReviewScoresSessionItem.CurrentRound.WinningGameId = winningGameId;
                sessionData.ReviewScoresSessionItem.CurrentRound.UserChoiceId = userChoiceId;

                // Update highest streak 
                if (sessionData.ReviewScoresSessionItem.Streak > sessionData.ReviewScoresSessionItem.HighestStreak)
                    sessionData.ReviewScoresSessionItem.HighestStreak = sessionData.ReviewScoresSessionItem.Streak;

                // Streak
                bool isCorrect = winningGameId == null || winningGameId == userChoiceId;
                if (!isCorrect)
                {
                    // Reset on incorrect
                    sessionData.ReviewScoresSessionItem.ReviewScoresRounds.Clear();
                }

                _sessionService.SetSessionData(sessionData, HttpContext);

                result = new
                {
                    winningGameId = sessionData.ReviewScoresSessionItem.CurrentRound.WinningGameId,

                    gameOneMetacriticScore = gameOne.MetacriticScore,
                    gameOneUserScore = gameOne.AverageUserScore,
                    gameOneScore = gameOne.GetAverageOverallRating(),

                    gameTwoMetacriticScore = gameTwo.MetacriticScore,
                    gameTwoUserScore = gameTwo.AverageUserScore,
                    gameTwoScore = gameTwo.GetAverageOverallRating()
                };
            }

            return Json(result);
        }

        public async Task<IActionResult> Index()
        {
            // Try load existing session data or create a new one
            var sessionData = _sessionService.GetSessionData(HttpContext);
            
            if (sessionData.ReviewScoresSessionItem.CurrentRound == null)
                await StartNewRoundAsync(sessionData.ReviewScoresSessionItem);

            var reviewScoresViewModel = await GetViewModelFromSessionDataAsync(sessionData);
            return View(reviewScoresViewModel);
        }

        public IActionResult GetImage(string imageName)
        {
            var imageUri = Path.Combine(_webHostEnvironment.WebRootPath, "images", imageName);
            if (System.IO.File.Exists(imageUri))
            {
                var imageBytes = System.IO.File.ReadAllBytes(imageUri);
                return File(imageBytes, "image/png");
            }
            else
                return NotFound();
        } 
        #endregion Actions..

        private async Task<ReviewScoresViewModel> GetViewModelFromSessionDataAsync(SessionData sessionData)
        {
            var systemStatus = await _systemStatusRepository.GetCurrentStatusAsync();

            var gameOneData = await _rawgGamesRepository.GetGameFromGameIdAsync(sessionData.ReviewScoresSessionItem.CurrentRound.GameOneId);
            var gameTwoData = await _rawgGamesRepository.GetGameFromGameIdAsync(sessionData.ReviewScoresSessionItem.CurrentRound.GameTwoId);

            ReviewScoresViewModel reviewScoresViewModel = new ReviewScoresViewModel()
            {
                SessionId = sessionData.SessionId,
                LastUpdateOn = systemStatus.Rawg_UpdatedOnUtc ?? DateTime.MinValue,
                HighestStreak = sessionData.ReviewScoresSessionItem.HighestStreak,
                Streak = sessionData.ReviewScoresSessionItem.Streak,

                GameOneName = gameOneData.Name,
                GameTwoName = gameTwoData.Name,

                GameOneImageUri = gameOneData.ImageUri,
                GameTwoImageUri = gameTwoData.ImageUri,

                CurrentRound = sessionData.ReviewScoresSessionItem.CurrentRound
            };

            return reviewScoresViewModel;
        }

        private async Task StartNewRoundAsync(ReviewScoresSessionItem reviewScoresSessionData)
        {
            List<RawgGame> games = await _rawgGamesRepository.GetRandomGamesAsync(2);

            if (games?.Any() ?? false)
            {
                reviewScoresSessionData.ReviewScoresRounds.Add(new ReviewScoresSessionItem.ReviewScoresRound()
                {
                    GameOneId = games[0].RawgGameId,
                    GameTwoId = games[1].RawgGameId
                });

                var sessionData = _sessionService.GetSessionData(HttpContext);
                _sessionService.SetSessionData(sessionData, HttpContext);
            }
        }

        private async Task<Guid?> GetWinningGameIdAsync(RawgGame gameOne, RawgGame gameTwo)
        {
            Guid? result = null;

            var gameOneAverageTotalRating = gameOne.GetAverageOverallRating();
            var gameTwoAverageTotalRating = gameTwo.GetAverageOverallRating();

            if (gameOneAverageTotalRating > gameTwoAverageTotalRating)
                result = gameOne.RawgGameId;
            else if (gameTwoAverageTotalRating > gameOneAverageTotalRating)
                result = gameTwo.RawgGameId;

            return result;
        }
        #endregion Methods..
    }
}
