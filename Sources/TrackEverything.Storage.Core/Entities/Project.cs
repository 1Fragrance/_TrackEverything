using System;
using System.Collections.Generic;

namespace TrackEverything.Storage.Core.Entities
{
    /// <summary>
    /// Project entity class for working with database
    /// </summary>
    public class Project
    {
        public Project()
        {
            Tasks = new List<Task>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreationDate { get; set; }

        public IEnumerable<Task> Tasks { get; set; }
    }
}