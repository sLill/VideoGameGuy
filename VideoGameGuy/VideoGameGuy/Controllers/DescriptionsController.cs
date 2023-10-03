using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using VideoGameGuy.Common;
using VideoGameGuy.Core;
using VideoGameGuy.Data;

namespace VideoGameGuy.Controllers
{
    public class DescriptionsController : Controller
    {
        #region Fields..
        public const int ROUND_TIME_LIMIT_MINUTES = 2;

        private readonly ILogger<DescriptionsController> _logger;
        private readonly ISessionService _sessionService;
        private readonly IIgdbGamesRepository _igdbGamesRepository;
        private readonly ISystemStatusRepository _systemStatusRepository;

        private static List<IgdbGame> _games;
        private Random _random = new Random();
        #endregion Fields..

        #region Constructors..
        public DescriptionsController(ILogger<DescriptionsController> logger,
                                      ISessionService sessionService,
                                      IIgdbGamesRepository igdbGamesRepository,
                                      ISystemStatusRepository systemStatusRepository)
        {
            _logger = logger;
            _sessionService = sessionService;
            _igdbGamesRepository = igdbGamesRepository;
            _systemStatusRepository = systemStatusRepository;
        }
        #endregion Constructors..

        #region Methods..
        #region Actions..
        public async Task<IActionResult> Index()
        {
            // Try load existing session data or create a new one
            var descriptionsSessionData = _sessionService.GetSessionData<DescriptionsSessionData>(HttpContext);
            if (descriptionsSessionData == null)
                descriptionsSessionData = new DescriptionsSessionData();

            if (descriptionsSessionData.CurrentRound == null)
                await StartNewRoundAsync(descriptionsSessionData);

            DescriptionsViewModel descriptionsViewModel = await GetViewModelFromSessionDataAsync(descriptionsSessionData);
            return View(descriptionsViewModel);
        }

        [HttpPost]
        public IActionResult GoNext()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Skip()
        {
            var descriptionsSessionData = _sessionService.GetSessionData<DescriptionsSessionData>(HttpContext);
            descriptionsSessionData.CurrentRound.IsSkipped = true;

            await StartNewRoundAsync(descriptionsSessionData);
            DescriptionsViewModel descriptionsViewModel = await GetViewModelFromSessionDataAsync(descriptionsSessionData);

            return RedirectToAction("Index", descriptionsViewModel);
        }

        public async Task<ActionResult> Validate()
        {
            var descriptionsSessionData = _sessionService.GetSessionData<DescriptionsSessionData>(HttpContext);
            if (descriptionsSessionData.CurrentRound != null)
            {
                descriptionsSessionData.CurrentRound.IsSolved = true;
                _sessionService.SetSessionData(descriptionsSessionData, HttpContext);
            }

            return Json(new { descriptionsSessionData.CurrentScore });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        #endregion Actions..

        public async Task<DescriptionsViewModel> GetViewModelFromSessionDataAsync(DescriptionsSessionData descriptionsSessionData)
        {
            // Load viewmodel from existing session data
            DescriptionsViewModel descriptionsViewModel = new DescriptionsViewModel() { SessionId = descriptionsSessionData.SessionId };

            var systemStatus = await _systemStatusRepository.GetCurrentStatusAsync();
            descriptionsViewModel.LastUpdateOn = systemStatus.Igdb_UpdatedOnUtc ?? DateTime.MinValue;

            foreach (var round in descriptionsSessionData.DescriptionsRounds)
            {
                descriptionsViewModel.DescriptionsRounds.Add(new DescriptionsRoundViewModel()
                {
                    GameTitle = round.GameTitle,
                    GameDescription = round.GameDescription,
                    IsSolved = round.IsSolved,
                    IsSkipped = round.IsSkipped,
                });
            }

            return descriptionsViewModel;
        }

        private async Task StartNewRoundAsync(DescriptionsSessionData descriptionsSessionData)
        {
            _games = _games ?? await _igdbGamesRepository.GetGamesWithStorylines(200);
            IgdbGame game = _games?.TakeRandom(1).FirstOrDefault();

            if (game != default)
            {
                descriptionsSessionData.DescriptionsRounds.Add(new DescriptionsSessionData.DescriptionsRound()
                {
                    GameTitle = game.Name,
                    GameDescription = game.Storyline,
                });
            }

            _sessionService.SetSessionData(descriptionsSessionData, HttpContext);
        }
        #endregion Methods..
    }
}
