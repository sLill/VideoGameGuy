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
            : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async override Task<bool> AddRangeAsync(IEnumerable<object> entities, bool suspendSaveChanges = false)
        {
            bool success = true;
            var platformLogos = new List<IgdbPlatformLogo>();

            try
            {
                foreach (var entity in entities)
                {
                    var platformLogo = new IgdbPlatformLogo();
                    platformLogo.Initialize((IgdbApiPlatformLogo)entity);
                    platformLogos.Add(platformLogo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            success &= await base.AddRangeAsync(platformLogos, suspendSaveChanges);
            return success;
        }

        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiPlatformLogo> apiPlatformLogos)
        {
            bool success = true;

            try
            {
                foreach (var apiPlatformLogo in apiPlatformLogos)
                {
                    var existingPlatformLogo = await _igdbDbContext.PlatformLogos.FirstOrDefaultAsync(x => x.SourceId == apiPlatformLogo.id);

                    // Add
                    if (existingPlatformLogo == default)
                    {
                        var platformLogo = new IgdbPlatformLogo();
                        platformLogo.Initialize(apiPlatformLogo);
                        _igdbDbContext.PlatformLogos.Add(platformLogo);
                    }

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
