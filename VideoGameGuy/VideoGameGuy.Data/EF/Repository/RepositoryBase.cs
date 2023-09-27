namespace VideoGameGuy.Data
{
    public abstract class RepositoryBase
    {
        #region Fields..
        protected readonly ILogger<RepositoryBase> _logger;

        protected List<object> _bulkItemsToAdd;
        protected List<object> _bulkItemsToUpdate;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public RepositoryBase(ILogger<RepositoryBase> logger)
        {
            _logger = logger;

            _bulkItemsToAdd = new List<object>();
            _bulkItemsToUpdate = new List<object>();
        }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
