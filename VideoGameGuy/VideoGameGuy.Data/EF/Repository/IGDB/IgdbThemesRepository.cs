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
            : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async override Task<bool> AddRangeAsync(IEnumerable<object> entities, bool suspendSaveChanges = false)
        {
            bool success = true;
            var themes = new List<IgdbTheme>();

            try
            {
                foreach (var entity in entities)
                {
                    var theme = new IgdbTheme();
                    theme.Initialize((IgdbApiTheme)entity);
                    themes.Add(theme);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            success &= await base.AddRangeAsync(themes, suspendSaveChanges);
            return success;
        }

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
        #endregion Methods..
    }
}
