using Microsoft.Data.SqlClient;

namespace VideoGameCritic.Core
{
    public class SqlLogger : ILogger
    {
        #region Fields..
        private readonly string _categoryName;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly string _connectionString;
        #endregion Fields..

        #region Constructors..
        public SqlLogger(string categoryName, Func<string, LogLevel, bool> filter, string connectionString)
        {
            _categoryName = categoryName;
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        #endregion Constructors..

        #region Methods..
        public IDisposable BeginScope<TState>(TState state) 
            => default;

        public bool IsEnabled(LogLevel logLevel) 
            => (_filter == null || _filter(_categoryName, logLevel));

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null)
                return;

            WriteLogToDatabase(logLevel, eventId, _categoryName, message, exception);
        }

        private void WriteLogToDatabase(LogLevel logLevel, EventId eventId, string category, string message, Exception exception)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO YourLogTable (LogLevel, EventId, Category, Message, Exception) VALUES (@logLevel, @eventId, @category, @message, @exception)", connection);

                command.Parameters.AddWithValue("@logLevel", logLevel.ToString());
                command.Parameters.AddWithValue("@eventId", eventId.Id);
                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@message", message);
                command.Parameters.AddWithValue("@exception", exception?.ToString() ?? "");

                connection.Open();
                command.ExecuteNonQuery();
            }
        } 
        #endregion Methods..
    }
}
