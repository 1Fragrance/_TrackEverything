namespace TrackEverything.Storage.Core.Interfaces
{
    /// <summary>
    /// UOW interface that contains all links to the repositories entities
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Interface that links with task repository
        /// </summary>
        ITaskRepository Tasks { get; }

        /// <summary>
        /// Interface that links with project repository
        /// </summary>
        IProjectRepository Projects { get; }

        /// <summary>
        /// Interface that links with worker repository
        /// </summary>
        IWorkerRepository Workers { get; }
    }
}