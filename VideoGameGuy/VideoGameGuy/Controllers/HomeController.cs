using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using VideoGameGuy.Common;
using VideoGameGuy.Data;

namespace VideoGameGuy.Controllers
{
    public class HomeController : Controller
    {
        #region Fields..
        private readonly ILogger<HomeController> _logger;
        private readonly ITrafficLogRepository _trafficLogRepository;

        private static Regex _refererRegex = new Regex(@"[Rr]eferer\=(?<Referer>[^$&]*)");
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

            if (headers.ContainsKey("Referer") && !string.IsNullOrEmpty(headers["Referer"]))
                referer = headers["Referer"];
            else
            {
                var queryString = HttpContext.Request.QueryString.ToString();
                var refererMatch = _refererRegex.Match(queryString);
                if (refererMatch.Success) 
                    referer = refererMatch.Groups["Referer"].Value;
            }

            if (headers.ContainsKey("User-Agent"))
                userAgent = headers["User-Agent"];

            await _trafficLogRepository.AddAsync(new TrafficLog()
            {
                Ip = ip,
                Referer = referer?.Truncate(500),
                UserAgent = userAgent?.Truncate(250)
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