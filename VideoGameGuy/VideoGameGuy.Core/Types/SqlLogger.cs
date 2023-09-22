using Microsoft.Data.SqlClient;
using VideoGameGuy.Data;

namespace VideoGameGuy.Core
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
            _connectionString = connectionString;
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

            var applicationLog = new ApplicationLog()
            {
                LogId = Guid.NewGuid(),
                LogLevel = logLevel.ToString(),
                EventId = eventId.Id,
                Category = _categoryName,
                Message = message,
                Exception = exception?.ToString() ?? string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                ModifiedOnUtc = DateTime.UtcNow
            };

            WriteApplicationLog(applicationLog);
        }

        private void WriteApplicationLog(ApplicationLog applicationLog)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO ApplicationLogs (LogId, LogLevel, EventId, Category, Message, Exception, CreatedOnUtc, ModifiedOnUtc) " +
                                             "VALUES (@LogId, @logLevel, @eventId, @category, @message, @exception, @createdOnUtc, @modifiedOnUtc)", connection);

                command.Parameters.AddWithValue("@LogId", applicationLog.LogId);
                command.Parameters.AddWithValue("@logLevel", applicationLog.LogLevel);
                command.Parameters.AddWithValue("@eventId", applicationLog.EventId);
                command.Parameters.AddWithValue("@category", applicationLog.Category);
                command.Parameters.AddWithValue("@message", applicationLog.Message);
                command.Parameters.AddWithValue("@exception", applicationLog.Exception);
                command.Parameters.AddWithValue("@createdOnUtc", applicationLog.CreatedOnUtc);
                command.Parameters.AddWithValue("@modifiedOnUtc", applicationLog.ModifiedOnUtc);

                connection.Open();
                command.ExecuteNonQuery();
            }
        } 
        #endregion Methods..
    }
}
