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
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiGameMode> apiGameModes, bool suspendSaveChanges = false)
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

                if (!suspendSaveChanges)
                    await _igdbDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            return success;
        }

        public async Task<bool> SaveBulkChangesAsync()
        {
            bool success = true;

            try
            {
                await _igdbDbContext.BulkUpdateAsync(_bulkItemsToUpdate);
                await _igdbDbContext.BulkInsertAsync(_bulkItemsToAdd);
                //await _igdbDbContext.BulkSaveChangesAsync();

                _bulkItemsToUpdate.Clear();
                _bulkItemsToAdd.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            return success;
        }

        public async Task<bool> StageBulkChangesAsync(IEnumerable<IgdbApiGameMode> apiGameModes)
        {
            bool success = true;

            foreach (var apiGameMode in apiGameModes)
            {
                try
                {
                    var existingGameMode = await _igdbDbContext.GameModes.FirstOrDefaultAsync(x => x.SourceId == apiGameMode.id);

                    // Add
                    if (existingGameMode == default)
                    {
                        var newItem = new IgdbGameMode();
                        newItem.Initialize(apiGameMode);
                        _bulkItemsToAdd.Add(newItem);
                    }

                    // Update
                    else
                    {
                        existingGameMode.Initialize(apiGameMode);
                        _bulkItemsToUpdate.Add(existingGameMode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                    success = false;
                }
            }

            return success;
        }
        #endregion Methods..
    }
}
