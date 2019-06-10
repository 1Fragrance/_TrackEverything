using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrackEverything.EFStorage.Context;
using TrackEverything.EFStorage.Entities;
using TrackEverything.Storage.Core.Entities;
using TrackEverything.Storage.Core.Interfaces;

namespace TrackEverything.EFStorage.Repositories
{
    /// <summary>
    /// Repository with CRUD methods of task entity
    /// using Entity Framework
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly ILogger logger;

        public TaskRepository(IMapper _mapper, ILogger log)
        {
            Mapper = _mapper;
            logger = log;
        }

        private IMapper Mapper { get; }

        public int Create(Task item)
        {
            if (item != null)
                try
                {
                    var temp = Mapper.Map<Task, EFTask>(item);
                    using (var db = new DatabaseContext())
                    {
                        db.Tasks.Attach(temp);
                        db.Entry(temp).State = EntityState.Added;
                        db.SaveChanges();
                        logger.LogInformation($"EF DATABASE: Added {item.Name} task");
                        return temp.Id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in adding {item.Name} task");
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
                        var tempTask = db.Tasks.Find(id);
                        var links = db.TaskWorkers
                            .Where(p => p.TaskId == id)
                            .ToList();
                        if (tempTask != null)
                        {
                            db.TaskWorkers.RemoveRange(links);
                            db.Tasks.Remove(tempTask);
                            db.SaveChanges();
                            logger.LogInformation($"EF DATABASE: Deleted task #{id}");
                        }
                        else
                        {
                            logger.LogWarning($"EF DATABASE: Can't find task with id #{id} for deleting");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in deleting task #{id}");
                    throw;
                }
        }

        public int Update(Task item, int id)
        {
            if (item != null && id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var links = db.TaskWorkers
                            .Where(p => p.TaskId == item.Id)
                            .ToList();
                        db.TaskWorkers.RemoveRange(links);

                        var temp = Mapper.Map<Task, EFTask>(item);

                        db.Tasks.Attach(temp);
                        db.Entry(temp).State = EntityState.Modified;
                        db.SaveChanges();
                        logger.LogInformation($"EF DATABASE: Updated task #{id}");
                        return id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in updating task #{id}");
                    throw;
                }

            return 0;
        }

        public Task Get(int id)
        {
            if (id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var temp = db.Tasks
                            .Include(p => p.Project)
                            .Include(t => t.TaskWorkers)
                            .ThenInclude(v => v.Worker)
                            .SingleOrDefault(g => g.Id == id);

                        if (temp != null)
                            return Mapper.Map<EFTask, Task>(temp);

                        logger.LogWarning($"EF DATABASE: Can't find task with id #{id} for uploading");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in uploading task #{id}");
                    throw;
                }

            return null;
        }

        public IEnumerable<Task> GetAll()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    IEnumerable<EFTask> temp = db.Tasks
                        .Include(p => p.Project)
                        .Include(t => t.TaskWorkers)
                        .ThenInclude(v => v.Worker);      
                    if (temp != null)
                    {
                        logger.LogInformation("EF DATABASE: Uploaded all tasks");
                        return Mapper.Map<IEnumerable<EFTask>, IEnumerable<Task>>(temp.ToList());
                    }

                    logger.LogWarning("EF DATABASE: No tasks found");
                    return new List<Task>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                logger.LogCritical("EF DATABASE: Error in uploading all tasks");
                throw;
            }
        }

        public IEnumerable<Worker> GetWorkersByTaskId(int id)
        {
            if (id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var temp = db.TaskWorkers
                            .Where(k => k.TaskId == id)
                            .Select(g => g.Worker)
                            .ToList(); 

                        if (temp != null)
                            return Mapper.Map<IEnumerable<EFWorker>, IEnumerable<Worker>>(temp);
                        return new List<Worker>();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in uploading task #{id} executors");
                    throw;
                }

            return new List<Worker>();
        }
    }
}