using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public class IgdbMultiplayerModesRepository : RepositoryBase, IIgdbMultiplayerModesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbMultiplayerModesRepository(ILogger<IgdbMultiplayerModesRepository> logger,
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
            var multiplayerModes = new List<IgdbMultiplayerMode>();

            try
            {
                foreach (var entity in entities)
                {
                    var multiplayerMode = new IgdbMultiplayerMode();
                    multiplayerMode.Initialize((IgdbApiMultiplayerMode)entity);
                    multiplayerModes.Add(multiplayerMode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            success &= await base.AddRangeAsync(multiplayerModes, suspendSaveChanges);
            return success;
        }

        public async Task<bool> AddOrUpdateRangeAsync(IEnumerable<IgdbApiMultiplayerMode> apiMultiplayerModes, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                foreach (var apiMultiplayerMode in apiMultiplayerModes)
                {
                    var existingMultiplayerMode = await _igdbDbContext.MultiplayerModes.FirstOrDefaultAsync(x => x.SourceId == apiMultiplayerMode.id);

                    // Add
                    if (existingMultiplayerMode == default)
                    {
                        var multiplayerMode = new IgdbMultiplayerMode();
                        multiplayerMode.Initialize(apiMultiplayerMode);
                        _igdbDbContext.MultiplayerModes.Add(multiplayerMode);
                    }

                    // Update
                    else
                    {
                        existingMultiplayerMode.Initialize(apiMultiplayerMode);
                        _igdbDbContext.MultiplayerModes.Update(existingMultiplayerMode);
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
