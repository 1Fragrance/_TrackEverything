using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TrackEverything.EFStorage.Context;
using TrackEverything.EFStorage.Entities;
using TrackEverything.Storage.Core.Entities;
using TrackEverything.Storage.Core.Interfaces;

namespace TrackEverything.EFStorage.Repositories
{
    /// <summary>
    /// Repository with CRUD methods of project entity
    /// using Entity Framework
    /// </summary>
    public class ProjectRepository : IProjectRepository
    {
        private readonly ILogger logger;

        public ProjectRepository(IMapper _mapper, ILogger log)
        {
            Mapper = _mapper;
            logger = log;
        }

        private IMapper Mapper { get; }

        public int Create(Project item)
        {
            if (item != null)
                try
                {
                    var temp = AutoMapper.Mapper.Map<Project, EFProject>(item);
                    using (var db = new DatabaseContext())
                    {
                        db.Projects.Attach(temp);
                        db.Entry(temp).State = EntityState.Added;
                        db.SaveChanges();
                        logger.LogInformation($"EF DATABASE: Added {item.Name} project");
                        return temp.Id;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"EF DATABASE: Error in adding {item.Name} project");
                    throw;
                }

            return 0;
        }

        public void Delete(int id)
        {
            if (id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var tempProject = db.Projects.Include(p => p.Tasks).SingleOrDefault(p => p.Id == id);

                        if (tempProject != null)
                        {
                            db.Projects.Remove(tempProject);
                            db.SaveChanges();
                            logger.LogInformation($"EF DATABASE: Deleted project #{id}");
                        }
                        else
                        {
                            logger.LogWarning($"EF DATABASE: Can't find project with id #{id} for deleting");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in deleting project #{id}");
                    throw;
                }
        }

        public int Update(Project item, int id)
        {
            if (item != null && id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var obj = db.Projects.SingleOrDefault(p => p.Id == item.Id);
                        if (obj != null)
                        {
                            db.Entry(obj).CurrentValues.SetValues(item);
                            db.SaveChanges();
                            logger.LogInformation($"EF DATABASE: Updated project #{id}");
                            return id;
                        }
                        else
                        {
                            logger.LogWarning($"EF DATABASE: Can't find project with id #{id} for updating");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in updating project #{id}");
                    throw;
                }

            return 0;
        }

        public Project Get(int id)
        {
            if (id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var temp = db.Projects.Include(p => p.Tasks).SingleOrDefault(g => g.Id == id);
                        if (temp != null)
                            return AutoMapper.Mapper.Map<EFProject, Project>(temp);

                        logger.LogWarning($"EF DATABASE: Can't find project with id #{id} for uploading");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in uploading project #{id}");
                    throw;
                }

            return null;
        }

        public IEnumerable<Project> GetAll()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    IEnumerable<EFProject> temp = db.Projects.Include(p => p.Tasks);
                    if (temp != null)
                    {
                        logger.LogInformation("EF DATABASE: Uploaded all projects");
                        return AutoMapper.Mapper.Map<IEnumerable<EFProject>, IEnumerable<Project>>(temp.ToList());
                    }

                    logger.LogWarning("EF DATABASE: No projects found");
                    return new List<Project>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                logger.LogCritical("EF DATABASE: Error in uploading all projects");
                throw;
            }
        }

        public IEnumerable<Task> GetTasksByProjectId(int id)
        {
            if (id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var temp = db.Tasks.Where(p => p.Id == id).ToList();
                        if (temp != null)
                            return AutoMapper.Mapper.Map<IEnumerable<EFTask>, IEnumerable<Task>>(temp);
                        return new List<Task>();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in uploading project #{id} tasks");
                    throw;
                }

            return new List<Task>();
        }
    }
}