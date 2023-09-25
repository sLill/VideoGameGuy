using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbPlatformLogosRepository : RepositoryBase, IIgdbPlatformLogosRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbPlatformLogosRepository(ILogger<IgdbPlatformLogosRepository> logger,
                                           IgdbDbContext igdbDbContext)
            : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformLogo> apiPlatformLogos)
        {
            bool success = true;

            try
            {
                foreach (var apiPlatformLogo in apiPlatformLogos)
                {
                    var existingPlatformLogo = await _igdbDbContext.PlatformLogos.FirstOrDefaultAsync(x => x.IgdbPlatformLogoId == apiPlatformLogo.id);

                    // Add
                    if (existingPlatformLogo == default)
                        _igdbDbContext.PlatformLogos.Add(new IgdbPlatformLogo(apiPlatformLogo));

                    // Update
                    else
                    {
                        existingPlatformLogo.Initialize(apiPlatformLogo);
                        _igdbDbContext.PlatformLogos.Update(existingPlatformLogo);
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
