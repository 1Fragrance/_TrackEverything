using Microsoft.Extensions.Logging;

namespace TrackEverything.Tools.Logger
{
    /// <summary>
    /// Custom logger configuration
    /// </summary>
    public class CustomLoggerProviderConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = 0;
    }
}