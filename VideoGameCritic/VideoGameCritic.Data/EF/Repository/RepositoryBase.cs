namespace VideoGameCritic.Data
{
    public abstract class RepositoryBase
    {
        #region Fields..
        protected readonly ILogger<RepositoryBase> _logger;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public RepositoryBase(ILogger<RepositoryBase> logger)
        {
            _logger = logger;
        }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
