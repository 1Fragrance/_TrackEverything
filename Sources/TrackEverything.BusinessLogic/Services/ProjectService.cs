using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.Storage.Core.Entities;
using TrackEverything.Storage.Core.Interfaces;

namespace TrackEverything.BusinessLogic.Services
{
    /// <summary>
    /// Contains all business logic methods of project entity
    /// </summary>
    public class ProjectService : IService<ProjectBO>
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, ILogger log)
        {
            this.mapper = mapper;
            Db = unitOfWork;
            logger = log;
        }

        private IUnitOfWork Db { get; }

        public int Add(ProjectBO newProject)
        {
            if (newProject != null)
            {
                try
                {
                    var project = mapper.Map<ProjectBO, Project>(newProject);
                    return Db.Projects.Create(project);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"SERVICE: Error in adding {newProject.Name} project");
                    throw;
                }
            }
            return 0;
        }

        public void Delete(int id)
        {
            if (id != 0)
            {
                try
                {
                    Db.Projects.Delete(id);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"SERVICE: Error in deleting project #{id}");
                    throw;
                }
            }
        }

        public ProjectBO Get(int id)
        {
            try
            {
                if (id != 0)
                {
                    var projectStorage = Db.Projects.Get(id);
                    var project = mapper.Map<Project, ProjectBO>(projectStorage);
                    return project;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in uploading project #{id}");
                throw;
            }
        }

        public IEnumerable<ProjectBO> GetAll()
        {
            try
            {
                var projects = Db.Projects.GetAll();
                if (projects != null)
                    return mapper.Map<IEnumerable<Project>, IEnumerable<ProjectBO>>(projects.ToList());
                return new List<ProjectBO>();
            }
            catch (Exception ex)
            {
                logger.LogCritical("SERVICE: Error in uploading all projects");
                throw;
            }
        }

        public int Update(ProjectBO project, int id)
        {
            try
            {
                if (project != null)
                {
                    var item = mapper.Map<ProjectBO, Project>(project);
                    return Db.Projects.Update(item, id);
                }

                return 0;
            }
            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in updating project #{id}");
                throw;
            }
        }
    }
}