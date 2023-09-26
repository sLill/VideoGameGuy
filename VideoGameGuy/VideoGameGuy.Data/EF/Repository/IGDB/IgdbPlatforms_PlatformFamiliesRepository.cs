using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbPlatforms_PlatformFamiliesRepository : RepositoryBase, IIgdbPlatforms_PlatformFamiliesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbPlatforms_PlatformFamiliesRepository(ILogger<IgdbPlatforms_PlatformFamiliesRepository> logger,
                                                        IgdbDbContext igdbDbContext)
         : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbPlatforms_PlatformFamilies> igdbPlatforms_PlatformFamilies, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var entry in igdbPlatforms_PlatformFamilies)
                {
                    var existingEntry = await _igdbDbContext.Platforms_PlatformFamilies.FirstOrDefaultAsync(x => x.Platforms_SourceId == entry.Platforms_SourceId
                                && x.PlatformFamilies_SourceId == entry.PlatformFamilies_SourceId);

                    // Add
                    if (existingEntry == default)
                        _igdbDbContext.Platforms_PlatformFamilies.Add(entry);

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
