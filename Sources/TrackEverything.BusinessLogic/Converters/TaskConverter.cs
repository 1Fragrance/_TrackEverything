using System;
using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.Converters
{
    /// <summary>
    /// Converter that converts task storage entity
    /// to the task business object
    /// </summary>
    public class TaskConverter : IConverter<Task, TaskBO>
    {
        public TaskBO Convert(Task _task)
        {
            if (_task != null)
            {
                var task = new TaskBO
                {
                    Id = _task.Id,
                    Name = _task.Name,
                    Estimation = TimeSpan.FromTicks(_task.Estimation),
                    Status = _task.Status,
                    CreationDate = _task.CreationDate,
                    StartAt = _task.StartAt,
                    EndAt = _task.EndAt,
                    ProjectId = _task.ProjectId
                };

                if (_task.Executors != null)
                {
                    var workerDalToBoConverter = new WorkerConverter();
                    var tempWorkers = new List<WorkerBO>();

                    foreach (var worker in _task.Executors)
                    {
                        var _worker = workerDalToBoConverter.Convert(worker);
                        _worker.Tasks = null;
                        tempWorkers.Add(_worker);

                    }
                    task.Workers = tempWorkers;
                }

                if (_task.Project != null)
                {
                    var projectDalToBoConverter = new ProjectConverter();
                    var _project = projectDalToBoConverter.Convert(_task.Project);

                    _project.Tasks = null;
                    task.Project = _project;
                }

                return task;
            }

            return null;
        }
    }
}