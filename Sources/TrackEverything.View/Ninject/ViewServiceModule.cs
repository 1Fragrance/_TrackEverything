using AutoMapper;
using Microsoft.Extensions.Logging;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using TrackEverything.BusinessLogic.AutomapperProfiles;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.BusinessLogic.Services;
using TrackEverything.Tools.Logger;
using TrackEverything.View.Controllers;
using TrackEverything.View.Converters;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Ninject
{
    /// <summary>
    /// Ninject Module that binds services,
    /// mapper and logger classes with appropriate interfaces
    /// </summary>
    public class ViewServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IService<WorkerBO>)).To(typeof(WorkerService));
            Bind(typeof(IService<TaskBO>)).To(typeof(TaskService));
            Bind(typeof(IService<ProjectBO>)).To(typeof(ProjectService));

            Bind<IMapper>().ToMethod(AutoMapper).InSingletonScope();
            Bind<ILogger>().To<CustomLogger>()
                .WithConstructorArgument("name", x => x.Request.ParentContext.Request.Service.FullName);

            Bind(typeof(IConverter<WorkerBO, WorkerViewModel>)).To(typeof(WorkerBOConverter));
            Bind(typeof(IConverter<WorkerViewModel, WorkerBO>)).To(typeof(WorkerViewConverter));

            Bind(typeof(IConverter<TaskBO, TaskViewModel>)).To(typeof(TaskBOConverter));
            Bind(typeof(IConverter<TaskViewModel, TaskBO>)).To(typeof(TaskViewConverter));

            Bind(typeof(IConverter<ProjectBO, ProjectViewModel>)).To(typeof(ProjectBOConverter));
            Bind(typeof(IConverter<ProjectViewModel, ProjectBO>)).To(typeof(ProjectViewConverter));
        }


        private IMapper AutoMapper(IContext context)
        {
            Mapper.Initialize(config =>
            {
                config.ConstructServicesUsing(type => context.Kernel.Get(type));

                config.AddProfile(new WorkerProfile());
                config.AddProfile(new TaskProfile());
                config.AddProfile(new ProjectProfile());
            });

            return Mapper.Instance;
        }
    }
}