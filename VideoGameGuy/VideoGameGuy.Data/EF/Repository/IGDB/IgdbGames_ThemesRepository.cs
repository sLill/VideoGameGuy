using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbGames_ThemesRepository : RepositoryBase, IIgdbGames_ThemesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGames_ThemesRepository(ILogger<IgdbGames_ThemesRepository> logger,
                                          IgdbDbContext igdbDbContext)
         : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Themes> igdbGames_Themes, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var entry in igdbGames_Themes)
                {
                    var existingEntry = await _igdbDbContext.Games_Themes.FirstOrDefaultAsync(x => x.Games_SourceId == entry.Games_SourceId 
                                && x.Themes_SourceId == entry.Themes_SourceId);

                    // Add
                    if (existingEntry == default)
                        _igdbDbContext.Games_Themes.Add(entry);

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
