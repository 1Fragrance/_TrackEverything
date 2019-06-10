using System;
using System.Collections.Generic;

namespace TrackEverything.BusinessLogic.BusinessObjects
{
    /// <summary>
    /// Project entity class for working with business logic
    /// </summary>
    public class ProjectBO
    {
        public ProjectBO()
        {
            Tasks = new List<TaskBO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreationDate { get; set; }

        public IEnumerable<TaskBO> Tasks { get; set; }
    }
}