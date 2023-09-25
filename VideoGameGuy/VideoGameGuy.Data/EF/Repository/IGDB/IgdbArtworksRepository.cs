using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbArtworksRepository : RepositoryBase, IIgdbArtworksRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbArtworksRepository(ILogger<IgdbArtworksRepository> logger,
                                      IgdbDbContext igdbDbContext)
            : base(logger)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiArtwork> apiArtworks)
        {
            bool success = true;

            try
            {
                foreach (var apiArtwork in apiArtworks)
                {
                    var existingArtwork = await _igdbDbContext.Artworks.FirstOrDefaultAsync(x => x.IgdbArtworkId == apiArtwork.id);

                    // Add
                    if (existingArtwork == default)
                        _igdbDbContext.Artworks.Add(new IgdbArtwork(apiArtwork));

                    // Update
                    else
                    {
                        existingArtwork.Initialize(apiArtwork);
                        _igdbDbContext.Artworks.Update(existingArtwork);
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
