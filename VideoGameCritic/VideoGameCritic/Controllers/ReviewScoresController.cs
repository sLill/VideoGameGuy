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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> btnGameOne_Click(Guid gameOneId)
        {
            Guid? winningGameId = await GetWinningGameIdAsync();

            if (winningGameId == null)
                return Redirect("http://youtube.com");
            if (winningGameId == gameOneId)
                return Redirect("http://google.com");
            else
                return Redirect("http://github.com");
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> btnGameTwo_Click(Guid gameTwoId)
        {
            Guid? winningGameId = await GetWinningGameIdAsync();

            if (winningGameId == null)
                return Redirect("http://youtube.com");
            if (winningGameId == gameTwoId)
                return Redirect("http://google.com");
            else
                return Redirect("http://github.com");
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            // Try load existing
            ReviewScoresViewModel reviewScoresViewModel = await GetViewModelFromSessionDataAsync();

            // Create new
            if (reviewScoresViewModel == null)
            {
                List<Game> games = await _gamesRepository.GetRandomGamesAsync(2);

                reviewScoresViewModel = new ReviewScoresViewModel() { GameOne = games[0], GameTwo = games[1] };
                var reviewScoresSessionData = new ReviewScoresSessionData() { GameOneId = reviewScoresViewModel.GameOne.GameId, GameTwoId = reviewScoresViewModel.GameTwo.GameId };

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

        public async Task<ReviewScoresViewModel> GetViewModelFromSessionDataAsync()
        {
            ReviewScoresViewModel reviewScoresViewModel = null;

            var reviewScoresSessionData = _sessionService.GetSessionData<ReviewScoresSessionData>(HttpContext);
            if (reviewScoresSessionData != null)
            {
                var gameOneData = await _gamesRepository.GetGameFromGameIdAsync(reviewScoresSessionData.GameOneId);
                var gameTwoData = await _gamesRepository.GetGameFromGameIdAsync(reviewScoresSessionData.GameTwoId);

                reviewScoresViewModel = new ReviewScoresViewModel() { GameOne = gameOneData, GameTwo = gameTwoData };
            }

            return reviewScoresViewModel;
        }

        private async Task<Guid?> GetWinningGameIdAsync()
        {
            Guid? result = null;

            var reviewScoresViewModel = await GetViewModelFromSessionDataAsync();

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
