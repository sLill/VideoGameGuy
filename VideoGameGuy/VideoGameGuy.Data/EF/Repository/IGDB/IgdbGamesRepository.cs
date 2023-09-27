using Microsoft.EntityFrameworkCore;
using VideoGameGuy.Common;

namespace VideoGameGuy.Data
{
    public class IgdbGamesRepository : RepositoryBase, IIgdbGamesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGamesRepository(ILogger<IgdbGamesRepository> logger,
                                   IgdbDbContext rawgDbContext)
            : base(logger)
        {
            _igdbDbContext = rawgDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateAsync(IgdbApiGame apiGame, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                var existingGame = await _igdbDbContext.Games.FirstOrDefaultAsync(x => x.SourceId == apiGame.id);

                // Add
                if (existingGame == default)
                {
                    var game = new IgdbGame();
                    game.Initialize(apiGame);
                    _igdbDbContext.Games.Add(game);
                }

                // Update
                else
                {
                    existingGame.Initialize(apiGame);
                    _igdbDbContext.Games.Update(existingGame);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            if (!suspendSaveChanges)
                await _igdbDbContext.SaveChangesAsync();

            return success;
        }

        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGame> apiGames)
        {
            bool success = true;

            foreach (var apiGame in apiGames)
                await AddOrUpdateAsync(apiGame, true);

            await _igdbDbContext.SaveChangesAsync();

            return success;
        }

        public async Task<List<IgdbGame>> GetGamesWithStorylinesAndMediaAsync(int minimumNumberOfRatings)
        {
            List<IgdbGame> games = default;

            try
            {
                if (_igdbDbContext.Games.Any())
                {
                    var eligibleGames = await (from g in _igdbDbContext.Games
                                               where g.Storyline != null
                                                  && g.Category == "main_game"
                                                  && g.TotalRating_Count != null
                                                  && g.TotalRating_Count >= minimumNumberOfRatings
                                                  && g.HasScreenshots
                                                  && g.HasArtworks
                                               select g).ToListAsync();

                    if (eligibleGames.Any())
                        games = eligibleGames;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return games;
        }

        public async Task<IgdbArtwork> GetArtworkFromGameAsync(IgdbGame game)
        {
            IgdbArtwork artwork = default;

            try
            {
                if (_igdbDbContext.Screenshots.Any())
                {
                    var artworks = await (from g in _igdbDbContext.Games
                                          join ga in _igdbDbContext.Games_Artworks on g.SourceId equals ga.Games_SourceId
                                          join a in _igdbDbContext.Artworks on ga.Artworks_SourceId equals a.SourceId
                                          select a).ToListAsync();

                    if (artworks.Any())
                        artwork = artworks.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return artwork;
        }

        public async Task<IgdbScreenshot> GetScreenshotFromGameAsync(IgdbGame game)
        {
            IgdbScreenshot screenshot = default;

            try
            {
                if (_igdbDbContext.Screenshots.Any())
                {
                    var screenshots = await (from g in _igdbDbContext.Games
                                             join gs in _igdbDbContext.Games_Screenshots on g.SourceId equals gs.Games_SourceId
                                             join s in _igdbDbContext.Screenshots on gs.Screenshots_SourceId equals s.SourceId
                                             select s).ToListAsync();

                    if (screenshots.Any())
                        screenshot = screenshots.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return screenshot;
        }
        #endregion Methods..
    }
}
