﻿using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbGames_GameModesRepository : RepositoryBase, IIgdbGames_GameModesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGames_GameModesRepository(ILogger<IgdbGames_GameModesRepository> logger,
                                             IgdbDbContext igdbDbContext)
         : base(logger, igdbDbContext)
        {
            _igdbDbContext = igdbDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbGames_GameModes> igdbGames_GameModes, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var entry in igdbGames_GameModes)
                {
                    var existingEntry = await _igdbDbContext.Games_GameModes.FirstOrDefaultAsync(x => x.Games_SourceId == entry.Games_SourceId 
                                && x.GameModes_SourceId == entry.GameModes_SourceId);

                    // Add
                    if (existingEntry == default)
                        _igdbDbContext.Games_GameModes.Add(entry);

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
