using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbPlatformsRepository : RepositoryBase, IIgdbPlatformsRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbPlatformsRepository(ILogger<IgdbPlatformsRepository> logger,
                                       IgdbDbContext igdbDbContext)
            : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatform> apiPlatforms, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var apiPlatform in apiPlatforms)
                {
                    var existingPlatform = await _igdbDbContext.Platforms.FirstOrDefaultAsync(x => x.SourceId == apiPlatform.id);

                    // Add
                    if (existingPlatform == default)
                    {
                        var platform = new IgdbPlatform();
                        platform.Initialize(apiPlatform);
                        _igdbDbContext.Platforms.Add(platform);
                    }

                    // Update
                    else
                    {
                        existingPlatform.Initialize(apiPlatform);
                        _igdbDbContext.Platforms.Update(existingPlatform);
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
