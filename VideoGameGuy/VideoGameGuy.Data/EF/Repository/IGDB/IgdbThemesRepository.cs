using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbThemesRepository : RepositoryBase, IIgdbThemesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbThemesRepository(ILogger<IgdbThemesRepository> logger,
                                    IgdbDbContext igdbDbContext)
            : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiTheme> apiThemes, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var apiTheme in apiThemes)
                {
                    var existingTheme = await _igdbDbContext.Themes.FirstOrDefaultAsync(x => x.SourceId == apiTheme.id);

                    // Add
                    if (existingTheme == default)
                    {
                        var theme = new IgdbTheme();
                        theme.Initialize(apiTheme);
                        _igdbDbContext.Themes.Add(theme);
                    }

                    // Update
                    else
                    {
                        existingTheme.Initialize(apiTheme);
                        _igdbDbContext.Themes.Update(existingTheme);
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

        public async Task<bool> SaveBulkChangesAsync()
        {
            bool success = true;

            try
            {
                await _igdbDbContext.BulkUpdateAsync(_bulkItemsToUpdate);
                await _igdbDbContext.BulkInsertAsync(_bulkItemsToAdd);
                //await _igdbDbContext.BulkSaveChangesAsync();

                _bulkItemsToUpdate.Clear();
                _bulkItemsToAdd.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            return success;
        }

        public async Task<bool> StageBulkChangesAsync(IEnumerable<IgdbApiTheme> apiThemes)
        {
            bool success = true;

            foreach (var apiTheme in apiThemes)
            {
                try
                {
                    var existingTheme = await _igdbDbContext.Themes.FirstOrDefaultAsync(x => x.SourceId == apiTheme.id);

                    // Add
                    if (existingTheme == default)
                    {
                        var newItem = new IgdbTheme();
                        newItem.Initialize(apiTheme);
                        _bulkItemsToAdd.Add(newItem);
                    }

                    // Update
                    else
                    {
                        existingTheme.Initialize(apiTheme);
                        _bulkItemsToUpdate.Add(existingTheme);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                    success = false;
                }
            }

            return success;
        }
        #endregion Methods..
    }
}
