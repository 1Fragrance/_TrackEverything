using AutoMapper;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.AutomapperProfiles
{
    /// <summary>
    /// Profile for AutoMapper that setup project conversions
    /// </summary>
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectBO>();
            CreateMap<ProjectBO, Project>();
        }
    }
}