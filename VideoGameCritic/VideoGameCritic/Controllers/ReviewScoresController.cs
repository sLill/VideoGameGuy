using Microsoft.AspNetCore.Mvc;
using VideoGameCritic.Core;
using VideoGameCritic.Data;

namespace VideoGameCritic.Controllers
{
    public class ReviewScoresController : Controller
    {
        #region Fields..
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISessionService _sessionService;
        private readonly IGamesRepository _gamesRepository;
        #endregion Fields..

        #region Constructors..
        public ReviewScoresController(IWebHostEnvironment webHostEnvironment,
                                      ISessionService sessionService,
                                      IGamesRepository gamesRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _sessionService = sessionService;
            _gamesRepository = gamesRepository;
        }
        #endregion Constructors..

        #region Methods..
        [HttpPost]
        public async Task<ActionResult> UserVote(Guid userChoiceId)
        {
            object? result = null;

            // Ignore vote if user has already voted for this set of games before
            var reviewScoresSessionData = _sessionService.GetSessionData<ReviewScoresSessionData>(HttpContext);
            if (reviewScoresSessionData.CurrentRound != null)
            {
                var reviewScoresViewModel = await GetViewModelFromSessionDataAsync(reviewScoresSessionData);
                Guid? winningGameId = await GetWinningGameIdAsync(reviewScoresViewModel);

                reviewScoresSessionData.CurrentRound.UserChoiceId = userChoiceId;
                _sessionService.SetSessionData(reviewScoresSessionData, HttpContext);

                // A null winningGameId indicates a tie (which counts as correct)
                result = new { IsCorrect = (winningGameId == null || userChoiceId == winningGameId) };
            }

            return Json(result);
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            // Try load existing session data
            var reviewScoresSessionData = _sessionService.GetSessionData<ReviewScoresSessionData>(HttpContext);
            if (reviewScoresSessionData == null)
            {
                // Create new session
                reviewScoresSessionData = new ReviewScoresSessionData();
                await StartNewRoundAsync(reviewScoresSessionData);
            }
            else if (reviewScoresSessionData.CurrentRound == null)
            {
                // Start a new round if the previous session ended on a complete round
                await StartNewRoundAsync(reviewScoresSessionData);
            }

            ReviewScoresViewModel reviewScoresViewModel = await GetViewModelFromSessionDataAsync(reviewScoresSessionData);
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

        public async Task<ReviewScoresViewModel> GetViewModelFromSessionDataAsync(ReviewScoresSessionData reviewScoresSessionData)
        {
            // Load viewmodel from existing session data
            ReviewScoresViewModel reviewScoresViewModel = new ReviewScoresViewModel();

            foreach (var gameRound in reviewScoresSessionData.GameRounds)
            {
                var gameOneData = await _gamesRepository.GetGameFromGameIdAsync(gameRound.GameOneId);
                var gameTwoData = await _gamesRepository.GetGameFromGameIdAsync(gameRound.GameTwoId);

                reviewScoresViewModel.GameRounds.Add(new GameRoundViewModel()
                {
                    GameOne = gameOneData,
                    GameTwo = gameTwoData,
                    UserChoice = gameRound.UserChoiceId
                });
            }

            return reviewScoresViewModel;
        }

        private async Task StartNewRoundAsync(ReviewScoresSessionData reviewScoresSessionData)
        {
            List<Game> games = await _gamesRepository.GetRandomGamesAsync(2);
            reviewScoresSessionData.GameRounds.Add(new ReviewScoresSessionData.GameRound()
            {
                GameOneId = games[0].GameId,
                GameTwoId = games[1].GameId
            });

            _sessionService.SetSessionData(reviewScoresSessionData, HttpContext);
        }

        private async Task<Guid?> GetWinningGameIdAsync(ReviewScoresViewModel reviewScoresViewModel)
        {
            Guid? result = null;

            var gameOneAverageTotalRating = reviewScoresViewModel.CurrentRound.GameOne.GetAverageOverallRating();
            var gameTwoAverageTotalRating = reviewScoresViewModel.CurrentRound.GameTwo.GetAverageOverallRating();

            if (gameOneAverageTotalRating > gameTwoAverageTotalRating)
                result = reviewScoresViewModel.CurrentRound.GameOne.GameId;
            else if (gameTwoAverageTotalRating > gameOneAverageTotalRating)
                result = reviewScoresViewModel.CurrentRound.GameTwo.GameId;

            return result;
        }
        #endregion Methods..
    }
}
