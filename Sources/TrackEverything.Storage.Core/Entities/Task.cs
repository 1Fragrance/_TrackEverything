using System;
using System.Collections.Generic;

namespace TrackEverything.Storage.Core.Entities
{
    /// <summary>
    /// Task entity class for working with database
    /// </summary>
    public class Task
    {
        public Task()
        {
            Executors = new List<Worker>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }

        public long Estimation { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public IEnumerable<Worker> Executors { get; set; }
    }
}