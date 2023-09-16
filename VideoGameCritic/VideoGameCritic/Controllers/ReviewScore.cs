using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoGameCritic.Data;

namespace VideoGameCritic.Controllers
{
    public class ReviewScore : Controller
    {
        #region Fields..
        private readonly IGamesRepository _gamesRepository;
        #endregion Fields..

        #region Constructors..
        public ReviewScore(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }
        #endregion Constructors..

        #region Methods..
        // GET: ReviewScore
        public ActionResult Index()
        {
            return View();
        } 
        #endregion Methods..
    }
}
