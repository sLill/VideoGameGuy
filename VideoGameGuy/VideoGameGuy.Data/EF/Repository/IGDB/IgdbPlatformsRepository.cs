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
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatform> apiPlatforms)
        {
            bool success = true;

            try
            {
                foreach (var apiPlatform in apiPlatforms)
                {
                    var existingPlatform = await _igdbDbContext.Platforms.FirstOrDefaultAsync(x => x.IgdbPlatformId == apiPlatform.id);

                    // Add
                    if (existingPlatform == default)
                        _igdbDbContext.Platforms.Add(new IgdbPlatform(apiPlatform));

                    // Update
                    else
                    {
                        existingPlatform.Initialize(apiPlatform);
                        _igdbDbContext.Platforms.Update(existingPlatform);
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
