using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbGames_ArtworksRepository : RepositoryBase, IIgdbGames_ArtworksRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGames_ArtworksRepository(ILogger<IgdbGames_ArtworksRepository> logger,
                                            IgdbDbContext igdbDbContext)
         : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_Artworks> igdbGames_Artworks, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var entry in igdbGames_Artworks)
                {
                    var existingEntry = await _igdbDbContext.Games_Artworks.FirstOrDefaultAsync(x => x.Games_SourceId == entry.Games_SourceId
                                && x.Artworks_SourceId == entry.Artworks_SourceId);

                    // Add
                    if (existingEntry == default)
                        _igdbDbContext.Games_Artworks.Add(entry);

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
