namespace VideoGameCritic.Data
{
    public class IgdbGamesRepository : RepositoryBase, IIgdbGamesRepository
    {
        #region Fields..
        protected readonly IgdbDbContext _igdbDbContext;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public IgdbGamesRepository(ILogger<IgdbGamesRepository> logger,
                                   IgdbDbContext rawgDbContext)
            : base(logger)
        {
            _igdbDbContext = rawgDbContext;
        }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
