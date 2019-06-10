using System.Collections.Generic;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.Storage.Core.Interfaces
{
    /// <summary>
    /// Project repository interface
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        /// <summary>
        /// Method that get project tasks by id
        /// </summary>
        IEnumerable<Task> GetTasksByProjectId(int id);
    }
}