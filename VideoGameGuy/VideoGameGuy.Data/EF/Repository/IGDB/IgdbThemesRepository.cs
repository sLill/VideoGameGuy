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
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiTheme> apiThemes)
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
