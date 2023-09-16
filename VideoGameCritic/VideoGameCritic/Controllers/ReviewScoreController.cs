using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoGameCritic.Data;

namespace VideoGameCritic.Controllers
{
    public class ReviewScoreController : Controller
    {
        #region Fields..
        private readonly IGamesRepository _gamesRepository;
        #endregion Fields..

        #region Constructors..
        public ReviewScoreController(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }
        #endregion Constructors..

        #region Methods..
        // GET: ReviewScore
        public async Task<IActionResult> Index()
        {
            List<Game> games = await _gamesRepository.GetRandomGamesAsync(2);


            return View();
        } 
        #endregion Methods..
    }
}
