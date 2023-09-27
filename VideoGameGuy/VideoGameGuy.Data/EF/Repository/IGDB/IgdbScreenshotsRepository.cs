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
            : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async override Task<bool> AddRangeAsync(IEnumerable<object> entities, bool suspendSaveChanges = false)
        {
            bool success = true;
            var screenshots = new List<IgdbScreenshot>();

            try
            {
                foreach (var entity in entities)
                {
                    var screenshot = new IgdbScreenshot();
                    screenshot.Initialize((IgdbApiScreenshot)entity);
                    screenshots.Add(screenshot);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            success &= await base.AddRangeAsync(screenshots, suspendSaveChanges);
            return success;
        }

        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiScreenshot> apiScreenshots, bool suspendSaveChanges = false)
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
