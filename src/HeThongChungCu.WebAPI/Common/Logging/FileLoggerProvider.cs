namespace HeThongChungCu.WebAPI.Common.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private static readonly object _lock = new object();

    public FileLoggerProvider(string filePath)
    {
        _filePath = filePath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, _filePath, _lock);
    }

    public void Dispose()
    {
    }
}

public class FileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _filePath;
    private readonly object _lock;

    public FileLogger(string categoryName, string filePath, object lockObj)
    {
        _categoryName = categoryName;
        _filePath = filePath;
        _lock = lockObj;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        var logRecord = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] [{_categoryName}] {message}";
        
        if (exception != null)
        {
            logRecord += Environment.NewLine + exception.ToString();
        }

        lock (_lock)
        {
            File.AppendAllText(_filePath, logRecord + Environment.NewLine);
        }
    }
}

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, string filePath)
    {
        builder.AddProvider(new FileLoggerProvider(filePath));
        return builder;
    }
}
