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
    /// Repository with CRUD methods of worker entity
    /// using Entity Framework
    /// </summary>
    public class WorkerRepository : IWorkerRepository
    {
        private readonly ILogger logger;

        public WorkerRepository(IMapper _mapper, ILogger log)
        {
            Mapper = _mapper;
            logger = log;
        }

        private IMapper Mapper { get; }

        public int Create(Worker item)
        {
            if (item != null)
                try
                {
                    var worker = Mapper.Map<Worker, EFWorker>(item);
                    using (var db = new DatabaseContext())
                    {
                        db.Workers.Attach(worker);
                        db.Entry(worker).State = EntityState.Added;
                        db.SaveChanges();
                        logger.LogInformation($"EF DATABASE: Added {item.Name} worker");
                        return worker.Id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in adding {item.Name} worker");
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
                        var tempWorker = db.Workers.Find(id);
                        var links = db.TaskWorkers
                            .Where(p => p.WorkerId == id)
                            .ToList();
                        if (tempWorker != null)
                        {
                            db.TaskWorkers.RemoveRange(links);
                            db.Workers.Remove(tempWorker);
                            db.SaveChanges();
                            logger.LogInformation($"EF DATABASE: Deleted worker #{id}");
                        }
                        else
                        {
                            logger.LogWarning($"EF DATABASE: Can't find worker with id #{id} for deleting");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in deleting worker #{id}");
                    throw;
                }
        }

        public Worker Get(int id)
        {
            if (id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var worker = db.Workers
                            .Include(t => t.TaskWorkers)
                            .ThenInclude(v => v.Task)
                            .SingleOrDefault(g => g.Id == id);

                        if (worker != null)
                            return Mapper.Map<EFWorker, Worker>(worker);

                        logger.LogWarning($"EF DATABASE: Can't find worker with id #{id} for uploading");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in uploading worker #{id}");
                    throw;
                }

            return null;
        }

        public IEnumerable<Worker> GetAll()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    IEnumerable<EFWorker> workers = db.Workers
                        .Include(t => t.TaskWorkers)
                        .ThenInclude(v => v.Task);
                        
                    if (workers != null)
                    {
                        logger.LogInformation("EF DATABASE: Uploaded all workers");
                        return Mapper.Map<IEnumerable<EFWorker>, IEnumerable<Worker>>(workers.ToList());
                    }

                    logger.LogWarning("EF DATABASE: No workers found");
                    return new List<Worker>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                logger.LogCritical("EF DATABASE: Error in uploading all workers");
                throw;
            }
        }

        public IEnumerable<Task> GetTasksByWorkerId(int id)
        {
            if (id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {                        
                        var temp = db.TaskWorkers
                            .Where(k => k.WorkerId == id)
                            .Select(g => g.Task)
                            .ToList();

                        if (temp != null)
                            return Mapper.Map<IEnumerable<EFTask>, IEnumerable<Task>>(temp);
                        return new List<Task>();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in uploading worker's #{id} tasks");
                    throw;
                }

            return new List<Task>();
        }

        public int Update(Worker item, int id)
        {
            if (item != null && id != 0)
                try
                {
                    using (var db = new DatabaseContext())
                    {
                        var links = db.TaskWorkers
                            .Where(p => p.WorkerId == item.Id)
                            .ToList();

                        db.TaskWorkers.RemoveRange(links);
                        var temp = Mapper.Map<Worker, EFWorker>(item);

                        db.Workers.Attach(temp);
                        db.Entry(temp).State = EntityState.Modified;
                        db.SaveChanges();
                        logger.LogInformation($"EF DATABASE: Updated worker #{id}");
                        return id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    logger.LogCritical($"EF DATABASE: Error in updating worker #{id}");
                    throw;
                }

            return 0;
        }
    }
}