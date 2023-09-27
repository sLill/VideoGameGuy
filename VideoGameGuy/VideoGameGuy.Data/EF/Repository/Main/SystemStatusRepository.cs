namespace VideoGameGuy.Data
{
    public class SystemStatusRepository : RepositoryBase, ISystemStatusRepository
    {
        #region Fields..
        private readonly MainDbContext _mainDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public SystemStatusRepository(ILogger<RawgGamesRepository> logger, 
                                      MainDbContext mainDbContext)
            : base(logger, mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<SystemStatus> GetCurrentStatusAsync()
        {
            SystemStatus systemStatus = null;

            try
            {
                systemStatus = _mainDbContext.SystemStatus.FirstOrDefault();
                if (systemStatus == default)
                {
                    systemStatus = new SystemStatus();
                    await AddAsync(systemStatus);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
            }

            return systemStatus;
        }

        public async Task<bool> AddAsync(SystemStatus systemStatus)
        {
            bool success = true;

            try
            {
                await _mainDbContext.SystemStatus.AddAsync(systemStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _mainDbContext.SaveChangesAsync();

            return success;
        }

        public async Task<bool> UpdateAsync(SystemStatus systemStatus)
        {
            bool success = true;

            try
            {
                _mainDbContext.Update(systemStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} - {ex.StackTrace}");
                success = false;
            }

            await _mainDbContext.SaveChangesAsync();
            return success;
        }
        #endregion Methods..
    }
}
