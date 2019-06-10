using System.Collections.Generic;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.Storage.Core.Interfaces
{
    /// <summary>
    /// Task repository interface
    /// </summary>
    public interface ITaskRepository : IRepository<Task>
    {
        /// <summary>
        /// Method that get task's workers by id
        /// </summary>
        IEnumerable<Worker> GetWorkersByTaskId(int id);
    }
}