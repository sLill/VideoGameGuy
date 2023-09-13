namespace VideoGameShowdown.Data
{
    public abstract class RepositoryBase
    {
        #region Fields..
        protected readonly ILogger<RepositoryBase> _logger;
        protected readonly ApplicationDbContext _applicationDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public RepositoryBase(ILogger<RepositoryBase> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
