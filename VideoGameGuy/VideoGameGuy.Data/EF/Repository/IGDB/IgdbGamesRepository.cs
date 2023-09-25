using Microsoft.EntityFrameworkCore;

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
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGame> apiGames)
        {
            bool success = true;

            foreach (var apiGame in apiGames)
            {
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
            }

            await _igdbDbContext.SaveChangesAsync();

            return success;
        }
        #endregion Methods..
    }
}
