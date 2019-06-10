using System.Collections.Generic;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.Storage.Core.Interfaces
{
    /// <summary>
    /// Worker repository interface
    /// </summary>
    public interface IWorkerRepository : IRepository<Worker>
    {
        /// <summary>
        /// Method that get worker's tasks by his id
        /// </summary>
        IEnumerable<Task> GetTasksByWorkerId(int id);
    }
}