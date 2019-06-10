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
    /// Contains all business logic methods of task entity
    /// </summary>
    public class TaskService : IService<TaskBO>
    {
        private readonly ILogger logger;
        private readonly IMapper mapper;

        public TaskService(IUnitOfWork unitOfWork, IMapper mapper, ILogger log)
        {
            this.mapper = mapper;
            Db = unitOfWork;
            logger = log;
        }

        private IUnitOfWork Db { get; }

        public int Add(TaskBO newTask)
        {
            if (newTask != null)
            {
                try
                {

                    var task = mapper.Map<TaskBO, Task>(newTask);
                    return Db.Tasks.Create(task);


                }
                catch (Exception ex)
                {
                    logger.LogCritical($"SERVICE: Error in adding {newTask.Name} task");
                    throw;
                }
            }

            return 0;
        }

        public void Delete(int id)
        {
            try
            {
                if (id != 0) Db.Tasks.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in deleting task #{id}");
                throw;
            }
        }

        public TaskBO Get(int id)
        {
            try
            {
                if (id != 0)
                {
                    var taskStorage = Db.Tasks.Get(id);
                    var task = mapper.Map<Task, TaskBO>(taskStorage);
                    return task;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in uploading task #{id}");
                throw;
            }
        }

        public IEnumerable<TaskBO> GetAll()
        {
            try
            {
                var tasksStorage = Db.Tasks.GetAll();
                if (tasksStorage != null)
                {
                    var tasks = mapper.Map<IEnumerable<Task>, IEnumerable<TaskBO>>(tasksStorage.ToList());
                    return tasks;
                }

                return new List<TaskBO>();
            }
            catch (Exception ex)
            {
                logger.LogInformation("SERVICE: Uploaded all tasks");
                throw;
            }
        }

        public int Update(TaskBO selectedTask, int id)
        {
            try
            {
                if (selectedTask != null)
                {
                    var task = mapper.Map<TaskBO, Task>(selectedTask);
                    Db.Tasks.Update(task, id);
                    return id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                logger.LogCritical($"SERVICE: Error in updating task #{id}");
                throw;
            }
        }
    }
}