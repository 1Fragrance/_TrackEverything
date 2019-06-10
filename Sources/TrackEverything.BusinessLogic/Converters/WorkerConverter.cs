using System;
using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.Converters
{
    /// <summary>
    /// Converter that converts worker storage entity
    /// to the worker business object
    /// </summary>
    public class WorkerConverter : IConverter<Worker, WorkerBO>
    {
        public WorkerBO Convert(Worker _worker)
        {
            if (_worker != null)
            {
                var worker = new WorkerBO
                {
                    Id = _worker.Id,
                    Name = _worker.Name,
                    Surname = _worker.Surname,
                    MiddleName = _worker.MiddleName,
                    Position = _worker.Position
                };

                if (_worker.Tasks != null)
                {
                    var tempTasks = new List<TaskBO>();
                    foreach (var taskDal in _worker.Tasks)
                    {
                        var taskDalToBoConverter = new TaskConverter();
                        var task = taskDalToBoConverter.Convert(taskDal);

                        task.Workers = null;
                        task.Project = null;
                        tempTasks.Add(task);
                    }
                    worker.Tasks = tempTasks;
                }

                return worker;
            }

            return null;
        }
    }
}