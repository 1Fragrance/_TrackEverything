using System.Linq;
using AutoMapper;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.EFStorage.Entities;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.AutomapperProfiles
{
    /// <summary>
    /// Profile for AutoMapper that setup worker conversions
    /// </summary>
    public class WorkerProfile : Profile
    {
        public WorkerProfile()
        {
            CreateMap<Worker, EFWorker>()
                .ForMember(p => p.TaskWorkers, opt => opt.MapFrom(u => u.Tasks))
                .AfterMap((model, entity) =>
                {
                    foreach (var entityTaskWorker in entity.TaskWorkers) entityTaskWorker.Worker = entity;
                });
            CreateMap<Task, EFTaskWorker>()
                .ForMember(entity => entity.Task, opt => opt.MapFrom(model => model))
                .ForMember(entity => entity.TaskId, opt => opt.MapFrom(model => model.Id));
            CreateMap<WorkerBO, Worker>()
                .ForMember(entity => entity.Tasks, opt => opt.MapFrom(model => model.Tasks));
            CreateMap<EFWorker, Worker>()
                .ForMember(v => v.Tasks, opt => opt.MapFrom(u => u.TaskWorkers.Select(y => y.Task).ToList()));
        }
    }
}