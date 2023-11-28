namespace VideoGameGuy.Data
{
    public class TrafficLogRepository : RepositoryBase, ITrafficLogRepository
    {
        #region Fields..
        private readonly MainDbContext _mainDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public TrafficLogRepository(ILogger<TrafficLogRepository> logger,
                                    MainDbContext mainDbContext)
            : base(logger, mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }
        #endregion Constructors..

        #region Methods..
        public async Task<bool> AddAsync(TrafficLog trafficLog)
        {
            bool success = true;

            try
            {
                await _mainDbContext.TrafficLogs.AddAsync(trafficLog);
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
