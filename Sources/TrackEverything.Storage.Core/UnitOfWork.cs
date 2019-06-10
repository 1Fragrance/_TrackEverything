using TrackEverything.Storage.Core.Interfaces;

namespace TrackEverything.Storage.Core
{
    /// <summary>
    ///     UOW class which connect all repositories in one entity.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IProjectRepository pr, ITaskRepository tr, IWorkerRepository wr)
        {
            Projects = pr;
            Tasks = tr;
            Workers = wr;
        }


        public IProjectRepository Projects { get; }

        public ITaskRepository Tasks { get; }

        public IWorkerRepository Workers { get; }
    }
}