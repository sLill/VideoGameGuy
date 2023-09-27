using Microsoft.EntityFrameworkCore;

namespace VideoGameGuy.Data
{
    public abstract class RepositoryBase
    {
        #region Fields..
        protected readonly ILogger<RepositoryBase> _logger;
        protected readonly DbContext _dbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public RepositoryBase(ILogger<RepositoryBase> logger, DbContext DbContext)
        {
            _logger = logger;
            _dbContext = DbContext;
        }
        #endregion Constructors..

        #region Methods..
        public virtual async Task<bool> AddRangeAsync(IEnumerable<object> entities, bool suspendSaveChanges = false)
        {
            bool success = true;

            try
            {
                await _dbContext.AddRangeAsync(entities);

                if (!suspendSaveChanges)
                    _dbContext.SaveChangesAsync();
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
