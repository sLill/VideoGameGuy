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
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> UserVote(Guid userChoiceId)
        {
            object? result = null;

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
            ReviewScoresViewModel reviewScoresViewModel = await GetViewModelFromSessionDataAsync(reviewScoresSessionData);

            // Create and store new session data
            if (reviewScoresViewModel == null)
            {
                List<Game> games = await _gamesRepository.GetRandomGamesAsync(2);

                reviewScoresViewModel = new ReviewScoresViewModel() { GameOne = games[0], GameTwo = games[1] };

                reviewScoresSessionData = new ReviewScoresSessionData();
                reviewScoresSessionData.GameRounds.Add(new ReviewScoresSessionData.GameRound() 
                { 
                    GameOneId = reviewScoresViewModel.GameOne.GameId, 
                    GameTwoId = reviewScoresViewModel.GameTwo.GameId 
                });

                _sessionService.SetSessionData(reviewScoresSessionData, HttpContext);
            }

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
            ReviewScoresViewModel reviewScoresViewModel = null;

            if (reviewScoresSessionData != null)
            {
                // Get the game ids from the most recent game round that the user has not voted on
                var gameOneData = await _gamesRepository.GetGameFromGameIdAsync(reviewScoresSessionData.CurrentRound.GameOneId);
                var gameTwoData = await _gamesRepository.GetGameFromGameIdAsync(reviewScoresSessionData.CurrentRound.GameTwoId);

                reviewScoresViewModel = new ReviewScoresViewModel() { GameOne = gameOneData, GameTwo = gameTwoData };
            }

            return reviewScoresViewModel;
        }

        private async Task<Guid?> GetWinningGameIdAsync(ReviewScoresViewModel reviewScoresViewModel)
        {
            Guid? result = null;

            var gameOneAverageTotalRating = reviewScoresViewModel.GameOne.GetAverageOverallRating();
            var gameTwoAverageTotalRating = reviewScoresViewModel.GameTwo.GetAverageOverallRating();

            if (gameOneAverageTotalRating > gameTwoAverageTotalRating)
                result = reviewScoresViewModel.GameOne.GameId;
            else if (gameTwoAverageTotalRating > gameOneAverageTotalRating)
                result = reviewScoresViewModel.GameTwo.GameId;

            return result;
        }
        #endregion Methods..
    }
}
