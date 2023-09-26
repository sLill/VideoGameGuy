using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbGames_PlatformsRepository : RepositoryBase, IIgdbGames_PlatformsRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGames_PlatformsRepository(ILogger<IgdbGames_PlatformsRepository> logger,
                                             IgdbDbContext igdbDbContext)
         : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Platforms> igdbGames_Platforms, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var entry in igdbGames_Platforms)
                {
                    var existingEntry = await _igdbDbContext.Games_Platforms.FirstOrDefaultAsync(x => x.Games_SourceId == entry.Games_SourceId 
                                && x.Platforms_SourceId == entry.Platforms_SourceId);

                    // Add
                    if (existingEntry == default)
                        _igdbDbContext.Games_Platforms.Add(entry);

                    // Update
                    else
                    {
                        // Nothing to update on join tables
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
        #endregion Methods..
    }
}
