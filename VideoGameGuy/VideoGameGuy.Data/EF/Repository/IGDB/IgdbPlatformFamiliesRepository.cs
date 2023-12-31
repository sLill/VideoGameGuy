using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbPlatformFamiliesRepository : RepositoryBase, IIgdbPlatformFamiliesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbPlatformFamiliesRepository(ILogger<IgdbPlatformFamiliesRepository> logger,
                                              IgdbDbContext igdbDbContext)
            : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async override Task<bool> AddRangeAsync(IEnumerable<object> entities, bool suspendSaveChanges = false)
        {
            bool success = true;
            var platformFamilies = new List<IgdbPlatformFamily>();

            try
            {
                foreach (var entity in entities)
                {
                    var platformFamily = new IgdbPlatformFamily();
                    platformFamily.Initialize((IgdbApiPlatformFamily)entity);
                    platformFamilies.Add(platformFamily);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            success &= await base.AddRangeAsync(platformFamilies, suspendSaveChanges);
            return success;
        }

        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformFamily> apiPlatformFamilies)
        {
            bool success = true;

            try
            {
                foreach (var apiPlatformFamily in apiPlatformFamilies)
                {
                    var existingPlatformFamily = await _igdbDbContext.PlatformFamilies.FirstOrDefaultAsync(x => x.SourceId == apiPlatformFamily.id);

                    // Add
                    if (existingPlatformFamily == default)
                    {
                        var platformFamily = new IgdbPlatformFamily();
                        platformFamily.Initialize(apiPlatformFamily);
                        _igdbDbContext.PlatformFamilies.Add(platformFamily);
                    }

                    // Update
                    else
                    {
                        existingPlatformFamily.Initialize(apiPlatformFamily);
                        _igdbDbContext.PlatformFamilies.Update(existingPlatformFamily);
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
