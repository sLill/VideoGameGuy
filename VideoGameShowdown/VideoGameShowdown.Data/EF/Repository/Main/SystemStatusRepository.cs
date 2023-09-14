namespace VideoGameShowdown.Data
{
    public class SystemStatusRepository : RepositoryBase, ISystemStatusRepository
    {
        #region Fields..
        private readonly MainDbContext _mainDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public SystemStatusRepository(ILogger<GamesRepository> logger, 
                                      MainDbContext mainDbContext)
            : base(logger)
        {
            _mainDbContext = mainDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public SystemStatus GetCurrentStatus()
            => _mainDbContext.SystemStatus.FirstOrDefault();

        public async Task UpdateAsync(SystemStatus status)
        { 
            _mainDbContext.Update(status);
            await _mainDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion Methods..
    }
}
