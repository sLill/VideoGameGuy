using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VideoGameGuy.Data;

namespace VideoGameGuy.Controllers
{
    public class HomeController : Controller
    {
        #region Fields..
        private readonly ILogger<HomeController> _logger;
        private readonly ITrafficLogRepository _trafficLogRepository;
        #endregion Fields..

        #region Constructors..
        public HomeController(ILogger<HomeController> logger,
                              ITrafficLogRepository trafficLogRepository)
        {
            _logger = logger;
            _trafficLogRepository = trafficLogRepository;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<IActionResult> Index()
        {
            await LogTraffic();
            return View();
        }

        private async Task LogTraffic()
        {
            var headers = HttpContext.Request.Headers;

            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            string referer = string.Empty;
            string userAgent = string.Empty;

            if (headers.ContainsKey("Referer"))
                referer = headers["Referer"];

            if (headers.ContainsKey("User-Agent"))
                userAgent = headers["User-Agent"];

            await _trafficLogRepository.AddAsync(new TrafficLog()
            {
                Ip = ip,
                Referer = referer,
                UserAgent = userAgent
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 
        #endregion Methods..
    }
}