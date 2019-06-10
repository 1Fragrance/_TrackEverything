using System.IO;
using Microsoft.Extensions.Configuration;

namespace TrackEverything.Storage.Core.Infrastructure
{
    /// <summary>
    /// Contains methods for working with database connection 
    /// </summary>
    public class SQLDataAccess
    {
        public SQLDataAccess()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false)
                .Build();

            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public string ConnectionString { get; set; }
    }
}