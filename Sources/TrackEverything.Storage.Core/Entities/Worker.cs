using System.Collections.Generic;

namespace TrackEverything.Storage.Core.Entities
{
    /// <summary>
    /// Worker entity class for working with database
    /// </summary>
    public class Worker
    {
        public Worker()
        {
            Tasks = new List<Task>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }

        public IEnumerable<Task> Tasks { get; set; }
    }
}