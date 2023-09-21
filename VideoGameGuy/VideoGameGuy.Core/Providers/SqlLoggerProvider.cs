namespace VideoGameGuy.Core
{
    public class SqlLoggerProvider : ILoggerProvider
    {
        #region Fields..
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly string _connectionString;
        #endregion Fields..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public SqlLoggerProvider(Func<string, LogLevel, bool> filter, string connectionString)
        {
            _filter = filter;
            _connectionString = connectionString;
        }
        #endregion Constructors..

        #region Methods..
        public ILogger CreateLogger(string categoryName)
            => new SqlLogger(categoryName, _filter, _connectionString);
        
        public void Dispose() { }
        #endregion Methods..
    }
}
