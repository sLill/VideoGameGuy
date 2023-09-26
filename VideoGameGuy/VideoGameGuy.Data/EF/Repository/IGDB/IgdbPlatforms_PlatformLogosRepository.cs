using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbPlatforms_PlatformLogosRepository : RepositoryBase, IIgdbPlatforms_PlatformLogosRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbPlatforms_PlatformLogosRepository(ILogger<IgdbPlatforms_PlatformLogosRepository> logger,
                                                     IgdbDbContext igdbDbContext)
         : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbPlatforms_PlatformLogos> igdbPlatforms_PlatformLogos, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var entry in igdbPlatforms_PlatformLogos)
                {
                    var existingEntry = await _igdbDbContext.Platforms_PlatformLogos.FirstOrDefaultAsync(x => x.Platforms_SourceId == entry.Platforms_SourceId
                                && x.PlatformLogos_SourceId == entry.PlatformLogos_SourceId);

                    // Add
                    if (existingEntry == default)
                        _igdbDbContext.Platforms_PlatformLogos.Add(entry);

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
