using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace VideoGameGuy.Controllers
{
    public class HomeController : Controller
    {
        #region Fields..
        private readonly ILogger<HomeController> _logger;
        #endregion Fields..

        #region Constructors..
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        #endregion Constructors..

        #region Methods..
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 
        #endregion Methods..
    }
}