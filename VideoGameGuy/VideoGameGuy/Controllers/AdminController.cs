using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace VideoGameGuy.Controllers
{
    public class AdminController : Controller
    {
        #region Fields..
        private const string CANARY_TOKEN_URL = @"http://canarytokens.com/stuff/tags/images/6t8qincmcj7sx8v1u573j7b4u/contact.php";

        private readonly ILogger<AdminController> _logger;
        #endregion Fields..

        #region Constructors..
        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }
        #endregion Constructors..

        #region Methods..
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return Redirect(CANARY_TOKEN_URL);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion Methods..
    }
}
