using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoGameCritic.Data;

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
        // GET: Index
        public async Task<IActionResult> Index()
        {
            List<Game> games = await _gamesRepository.GetRandomGamesAsync(2);
            return View(new ReviewScoresViewModel() { GameOne = games[0], GameTwo = games[1] });
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
