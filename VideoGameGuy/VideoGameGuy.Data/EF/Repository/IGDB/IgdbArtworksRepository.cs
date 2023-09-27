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
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiArtwork> apiArtworks, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var apiArtwork in apiArtworks)
                {
                    var existingArtwork = await _igdbDbContext.Artworks.FirstOrDefaultAsync(x => x.SourceId == apiArtwork.id);

                    // Add
                    if (existingArtwork == default)
                    {
                        var artwork = new IgdbArtwork();
                        artwork.Initialize(apiArtwork);
                        _igdbDbContext.Artworks.Add(artwork);
                    }

                    // Update
                    else
                    {
                        existingArtwork.Initialize(apiArtwork);
                        _igdbDbContext.Artworks.Update(existingArtwork);
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

        public async Task<bool> StageBulkChangesAsync(IEnumerable<IgdbApiArtwork> apiArtworks)
        {
            bool success = true;

            foreach (var apiArtwork in apiArtworks)
            {
                try
                {
                    var existingArtwork = await _igdbDbContext.Artworks.FirstOrDefaultAsync(x => x.SourceId == apiArtwork.id);

                    // Add
                    if (existingArtwork == default)
                    {
                        var newItem = new IgdbArtwork();
                        newItem.Initialize(apiArtwork);
                        _bulkItemsToAdd.Add(newItem);
                    }

                    // Update
                    else
                    {
                        existingArtwork.Initialize(apiArtwork);
                        _bulkItemsToUpdate.Add(existingArtwork);
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
