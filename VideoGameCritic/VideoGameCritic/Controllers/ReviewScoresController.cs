using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoGameCritic.Data;

namespace VideoGameCritic.Controllers
{
    public class ReviewScoresController : Controller
    {
        #region Fields..
        private readonly IGamesRepository _gamesRepository;
        #endregion Fields..

        #region Constructors..
        public ReviewScoresController(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }
        #endregion Constructors..

        #region Methods..
        // GET: GetModel
        public async Task<IActionResult> ReviewScores()
        {
            List<Game> games = await _gamesRepository.GetRandomGamesAsync(2);
            return View(new ReviewScoresViewModel() { GameOne = games[0], GameTwo = games[1] });
        }
        #endregion Methods..
    }
}
