using System.IO;
using Microsoft.Extensions.Configuration;

namespace TrackEverything.BusinessLogic.Infrastructure
{
    public class ProjectConfiguration
    {
        /// <summary>
        ///     Contains methods for checking selected configuration of the project.
        /// </summary>
        public ProjectConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false)
                .Build();

            if (configuration.GetSection("Database")["ADO.NET"].ToLower() == "true")
                DBState = "ADO.NET";
            else
                DBState = "EntityFramework";
        }

        public string DBState { get; set; }
    }
}