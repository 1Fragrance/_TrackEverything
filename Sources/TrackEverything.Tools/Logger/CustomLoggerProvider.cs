using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace TrackEverything.Tools.Logger
{
    /// <summary>
    /// Custom logger provider
    /// </summary>
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly CustomLoggerProviderConfiguration loggerConfig;

        private readonly ConcurrentDictionary<string, CustomLogger> loggers =
            new ConcurrentDictionary<string, CustomLogger>();

        public CustomLoggerProvider(CustomLoggerProviderConfiguration config)
        {
            loggerConfig = config;
        }

        public ILogger CreateLogger(string category)
        {
            return loggers.GetOrAdd(category, name => new CustomLogger(name, loggerConfig));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}