using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbGames_ScreenshotsRepository : RepositoryBase, IIgdbGames_ScreenshotsRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGames_ScreenshotsRepository(ILogger<IgdbGames_ScreenshotsRepository> logger,
                                               IgdbDbContext igdbDbContext)
         : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Screenshots> igdbGames_Screenshots, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var entry in igdbGames_Screenshots)
                {
                    var existingEntry = await _igdbDbContext.Games_Screenshots.FirstOrDefaultAsync(x => x.Games_SourceId == entry.Games_SourceId
                                && x.Screenshots_SourceId == entry.Screenshots_SourceId);

                    // Add
                    if (existingEntry == default)
                        _igdbDbContext.Games_Screenshots.Add(entry);

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
