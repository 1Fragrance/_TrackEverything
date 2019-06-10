using System;
using System.Collections.Generic;

namespace TrackEverything.BusinessLogic.BusinessObjects
{
    /// <summary>
    /// Task entity class for working with business logic
    /// </summary>
    public class TaskBO
    {
        public TaskBO()
        {
            Workers = new List<WorkerBO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }

        public TimeSpan Estimation { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public int ProjectId { get; set; }
        public ProjectBO Project { get; set; }

        public IEnumerable<WorkerBO> Workers { get; set; }
    }
}