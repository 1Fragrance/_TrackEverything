using System;
using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Converters
{
    /// <summary>
    /// Converter that converts worker view model
    /// to the worker business object
    /// </summary>
    public class WorkerViewConverter : IConverter<WorkerViewModel, WorkerBO>
    {
        public WorkerBO Convert(WorkerViewModel worker)
        {
            if (worker != null)
            {
                var workerBo = new WorkerBO
                {
                    Id = worker.Id,
                    Name = worker.Name,
                    Surname = worker.Surname,
                    MiddleName = worker.MiddleName,
                    Position = worker.Position,
                    Tasks = null
                };


                if (worker.Tasks != null)
                {
                    var tempTasks = new List<TaskBO>();
                    foreach (var taskView in worker.Tasks)
                    {
                        var taskViewToBoConverter = new TaskViewConverter();
                        var task = taskViewToBoConverter.Convert(taskView);

                        task.Workers = null;
                        task.Project = null;
                        tempTasks.Add(task);
                    }
                    workerBo.Tasks = tempTasks;
                }

                return workerBo;
            }

            return null;
        }
    }
}