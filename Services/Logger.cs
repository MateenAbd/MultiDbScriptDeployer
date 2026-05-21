using System;
using System.IO;
using System.Text;

namespace MultiDbScriptDeployer.Services
{
    public class Logger
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        public event EventHandler<LogEntry> LogAdded;

        public Logger()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDirectory);
            
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _logFilePath = Path.Combine(logDirectory, $"deployment_{timestamp}.log");
        }

        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public void LogSuccess(string message)
        {
            Log("SUCCESS", message);
        }

        public void LogWarning(string message)
        {
            Log("WARNING", message);
        }

        public void LogError(string message)
        {
            Log("ERROR", message);
        }

        public void LogError(string message, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine($"Exception: {ex.Message}");
            sb.AppendLine($"Stack Trace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
            }
            
            Log("ERROR", sb.ToString());
        }

        private void Log(string level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{level}] {message}";

            lock (_lockObject)
            {
                try
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to log file: {ex.Message}");
                }
            }

            // Raise event for UI updates with color information
            LogLevel logLevel = level switch
            {
                "ERROR" => LogLevel.Error,
                "SUCCESS" => LogLevel.Success,
                "WARNING" => LogLevel.Warning,
                _ => LogLevel.Info
            };

            LogAdded?.Invoke(this, new LogEntry { Message = logEntry, Level = logLevel });
        }

        public string GetLogFilePath()
        {
            return _logFilePath;
        }
    }

    public class LogEntry
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
    }

    public enum LogLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

}
