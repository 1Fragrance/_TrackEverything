using System.Collections.Generic;

namespace TrackEverything.BusinessLogic.BusinessObjects
{
    /// <summary>
    /// Worker entity class for working with business logic
    /// </summary>
    public class WorkerBO
    {
        public WorkerBO()
        {
            Tasks = new List<TaskBO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }

        public IEnumerable<TaskBO> Tasks { get; set; }
    }
}