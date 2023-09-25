using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbScreenshotsRepository : RepositoryBase, IIgdbScreenshotsRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbScreenshotsRepository(ILogger<IgdbScreenshotsRepository> logger,
                                         IgdbDbContext igdbDbContext)
            : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiScreenshot> apiScreenshots)
        {
            bool success = true;

            try
            {
                foreach (var apiScreenshot in apiScreenshots)
                {
                    var existingScreenshot = await _igdbDbContext.Screenshots.FirstOrDefaultAsync(x => x.SourceId == apiScreenshot.id);

                    // Add
                    if (existingScreenshot == default)
                    {
                        var screenshot = new IgdbScreenshot();
                        screenshot.Initialize(apiScreenshot);
                        _igdbDbContext.Screenshots.Add(screenshot);
                    }

                    // Update
                    else
                    {
                        existingScreenshot.Initialize(apiScreenshot);
                        _igdbDbContext.Screenshots.Update(existingScreenshot);
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
