using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbGameModesRepository : RepositoryBase, IIgdbGameModesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGameModesRepository(ILogger<IgdbGameModesRepository> logger,
                                       IgdbDbContext igdbDbContext)
            : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGameMode> apiGameModes)
        {
            bool success = true;

            try
            {
                foreach (var apiGameMode in apiGameModes)
                {
                    var existingGameModes = await _igdbDbContext.GameModes.FirstOrDefaultAsync(x => x.SourceId == apiGameMode.id);

                    // Add
                    if (existingGameModes == default)
                    {
                        var gameMode = new IgdbGameMode();
                        gameMode.Initialize(apiGameMode);
                        _igdbDbContext.GameModes.Add(gameMode);
                    }

                    // Update
                    else
                    {
                        existingGameModes.Initialize(apiGameMode);
                        _igdbDbContext.GameModes.Update(existingGameModes);
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
