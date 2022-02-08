namespace student_contest.api.ExceptionMiddleware
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "StudentContestApiLogs.txt");

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            File.AppendAllText(_filePath, formatter(state, exception) + Environment.NewLine);
        }
    }
}
