using Ninject.Modules;
using TrackEverything.ADOStorage.Repositories;
using TrackEverything.Storage.Core;
using TrackEverything.Storage.Core.Interfaces;

namespace TrackEverything.BusinessLogic.Infrastructure
{
    /// <summary>
    /// Ninject Module that binds repositories 
    /// classes with appropriate interfaces
    /// </summary>
    public class BLServiceModule : NinjectModule
    {
        public override void Load()
        {
            var configuration = new ProjectConfiguration().DBState;
            if (configuration == "ADO.NET")
            {
                Bind<IWorkerRepository>().To<WorkerRepository>();
                Bind<ITaskRepository>().To<TaskRepository>();
                Bind<IProjectRepository>().To<ProjectRepository>();

                Bind<IUnitOfWork>().To<UnitOfWork>();
            }
            else
            {
                Bind<IWorkerRepository>().To<EFStorage.Repositories.WorkerRepository>();
                Bind<ITaskRepository>().To<EFStorage.Repositories.TaskRepository>();
                Bind<IProjectRepository>().To<EFStorage.Repositories.ProjectRepository>();

                Bind<IUnitOfWork>().To<UnitOfWork>();
            }
        }
    }
}