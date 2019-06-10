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
    /// Contains all business logic methods of worker entity
    /// </summary>
    public class WorkerService : IService<WorkerBO>
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;

        public WorkerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger log)
        {
            this.mapper = mapper;
            Db = unitOfWork;
            logger = log;
        }

        public IUnitOfWork Db { get; set; }

        public int Add(WorkerBO newWorker)
        {
            if (newWorker != null)
            {
                try
                {
                    var worker = mapper.Map<WorkerBO, Worker>(newWorker);
                    return Db.Workers.Create(worker);
                }

                catch (Exception ex)
                {
                    logger.LogCritical($"SERVICE: Error in adding {newWorker.Name} worker");
                    throw;
                }
            }

            return 0;
        }

        public WorkerBO Get(int id)
        {
            try
            {
                if (id != 0)
                {
                    var workerStorage = Db.Workers.Get(id);
                    var worker = mapper.Map<Worker, WorkerBO>(workerStorage);
                    return worker;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in uploading worker #{id}");
                throw;
            }
        }

        public IEnumerable<WorkerBO> GetAll()
        {
            try
            {
                var workersStorage = Db.Workers.GetAll();
                if (workersStorage != null)
                {
                    var workers = mapper.Map<IEnumerable<Worker>, IEnumerable<WorkerBO>>(workersStorage.ToList());
                    return workers;
                }

                return new List<WorkerBO>();
            }
            catch (Exception ex)
            {
                logger.LogCritical("SERVICE: Error in uploading all workers");
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                if (id != 0)
                    Db.Workers.Delete(id);
            }

            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in deleting worker #{id}");
                throw;
            }
        }

        public int Update(WorkerBO selectedWorker, int id)
        {
            try
            {
                if (selectedWorker != null)
                {
                    var worker = mapper.Map<WorkerBO, Worker>(selectedWorker);
                    return Db.Workers.Update(worker, id);
                }

                return 0;
            }
            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in updating worker #{id}");
                throw;
            }
        }
    }
}