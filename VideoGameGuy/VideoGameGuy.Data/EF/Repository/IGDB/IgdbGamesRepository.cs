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

            try
            {
                foreach (var apiGame in apiGames)
                {
                    var existingGame = await _igdbDbContext.Games.FirstOrDefaultAsync(x => x.IgdbGameId == apiGame.id);

                    // Add
                    if (existingGame == default)
                        _igdbDbContext.Games.Add(new IgdbGame(apiGame));

                    // Update
                    else
                    {
                        existingGame.Initialize(apiGame);
                        _igdbDbContext.Games.Update(existingGame);
                    }
                }

                await _igdbDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            return success;
        }
        #endregion Methods..
    }
}
