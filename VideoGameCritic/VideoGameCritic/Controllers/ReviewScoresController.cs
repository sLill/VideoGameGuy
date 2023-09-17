using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoGameCritic.Data;
using Newtonsoft.Json;

namespace VideoGameCritic.Controllers
{
    public class ReviewScoresController : Controller
    {
        #region Fields..
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGamesRepository _gamesRepository;
        #endregion Fields..

        #region Constructors..
        public ReviewScoresController(IWebHostEnvironment webHostEnvironment,
                                      IGamesRepository gamesRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _gamesRepository = gamesRepository;
        }
        #endregion Constructors..

        #region Methods..
        [ValidateAntiForgeryToken]
        public IActionResult btnGameOne_Click(Game GameOne)
        {
            return Redirect("http://google.com");
        }

        [ValidateAntiForgeryToken]
        public IActionResult btnGameTwo_Click(Game GameTwo)
        {
            return Redirect("http://google.com");
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            ReviewScoresViewModel reviewScoresViewModel = null;

            var sessionDataString = HttpContext.Session.GetString(nameof(ReviewScoresSessionData));
            if (!string.IsNullOrEmpty(sessionDataString))
            {
                var reviewScoresSessionData = JsonConvert.DeserializeObject<ReviewScoresSessionData>(sessionDataString);

                var gameOneData = await _gamesRepository.GetGameFromGameIdAsync(reviewScoresSessionData.GameOneId);
                var gameTwoData = await _gamesRepository.GetGameFromGameIdAsync(reviewScoresSessionData.GameTwoId);

                reviewScoresViewModel = new ReviewScoresViewModel() { GameOne = gameOneData, GameTwo = gameTwoData };
            }
            else
            {
                List<Game> games = await _gamesRepository.GetRandomGamesAsync(2);

                reviewScoresViewModel = new ReviewScoresViewModel() { GameOne = games[0], GameTwo = games[1] };
                HttpContext.Session.SetString(nameof(ReviewScoresSessionData), JsonConvert.SerializeObject(new ReviewScoresSessionData() { GameOneId = reviewScoresViewModel.GameOne.GameId, GameTwoId = reviewScoresViewModel.GameTwo.GameId }));
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
        #endregion Methods..
    }
}
