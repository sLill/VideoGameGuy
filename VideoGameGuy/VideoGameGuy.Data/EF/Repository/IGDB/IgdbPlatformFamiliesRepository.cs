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
            : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformFamily> apiPlatformFamilies)
        {
            bool success = true;

            try
            {
                foreach (var apiPlatformFamily in apiPlatformFamilies)
                {
                    var existingPlatformFamily = await _igdbDbContext.PlatformFamilies.FirstOrDefaultAsync(x => x.IgdbPlatformFamilyId == apiPlatformFamily.id);

                    // Add
                    if (existingPlatformFamily == default)
                        _igdbDbContext.PlatformFamilies.Add(new IgdbPlatformFamily(apiPlatformFamily));

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
