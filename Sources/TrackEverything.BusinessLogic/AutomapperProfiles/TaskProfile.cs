using System;
using System.Linq;
using AutoMapper;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.EFStorage.Entities;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.AutomapperProfiles
{
    /// <summary>
    /// Profile for AutoMapper that setup task conversions
    /// </summary>
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Task, EFTask>()
                .ForMember(p => p.TaskWorkers, opt => opt.MapFrom(u => u.Executors))
                .AfterMap((model, entity) =>
                {
                    foreach (var entityTaskWorker in entity.TaskWorkers) entityTaskWorker.Task = entity;
                });
            CreateMap<Worker, EFTaskWorker>()
                .ForMember(entity => entity.Worker, opt => opt.MapFrom(model => model))
                .ForMember(entity => entity.WorkerId, opt => opt.MapFrom(model => model.Id));
            CreateMap<TaskBO, Task>()
                .ForMember(entity => entity.Executors, opt => opt.MapFrom(model => model.Workers))
                .ForMember(p => p.Estimation, opt => opt.MapFrom(t => t.Estimation.Ticks));
            CreateMap<Task, TaskBO>()
                .ForMember(entity => entity.Workers, opt => opt.MapFrom(model => model.Executors))
                .ForMember(p => p.Estimation, opt => opt.MapFrom(t => TimeSpan.FromTicks(t.Estimation)));
            CreateMap<EFTask, Task>()
                .ForMember(v => v.Executors, opt => opt.MapFrom(u => u.TaskWorkers.Select(y => y.Worker).ToList()));
        }
    }
}