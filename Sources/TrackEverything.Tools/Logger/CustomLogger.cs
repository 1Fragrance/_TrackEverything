using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TrackEverything.Tools.Logger
{
    /// <summary>
    /// Custom logger class which inherited from ILogger interface
    /// </summary>
    public class CustomLogger : ILogger
    {
        private readonly CustomLoggerProviderConfiguration loggerConfig;
        private readonly string loggerName;
        private readonly string pathToFile;

        public CustomLogger(string name, CustomLoggerProviderConfiguration config)
        {
            pathToFile = GetLoggerPath();
            loggerName = name;
            loggerConfig = config;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var message = string.Format("{0}: {1} - {2}", logLevel.ToString(), DateTime.UtcNow,
                formatter(state, exception));
            WriteTextToFile(message);
        }

        private void WriteTextToFile(string message)
        {
            using (var streamWriter = new StreamWriter(pathToFile, true))
            {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }
        }

        private string GetLoggerPath()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false)
                .Build();

            return configuration.GetSection("LogPath")["Path"];
        }
    }
}