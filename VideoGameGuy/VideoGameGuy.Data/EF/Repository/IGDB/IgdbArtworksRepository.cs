using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
            : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async override Task<bool> AddRangeAsync(IEnumerable<object> entities, bool suspendSaveChanges = false)
        {
            bool success = true;
            var artworks = new List<IgdbArtwork>();

            try
            {
                foreach (var entity in entities)
                {
                    var artwork = new IgdbArtwork();
                    artwork.Initialize((IgdbApiArtwork)entity);
                    artworks.Add(artwork);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            success &= await base.AddRangeAsync(artworks, suspendSaveChanges);
            return success;
        }

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
        #endregion Methods..
    }
}
