using System;
using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Converters
{
    /// <summary>
    /// Converter that converts task view model
    /// to the task business object
    /// </summary>
    public class TaskViewConverter : IConverter<TaskViewModel, TaskBO>
    {
        public TaskBO Convert(TaskViewModel _task)
        {
            if (_task != null)
            {
                var task = new TaskBO
                {
                    Id = _task.Id,
                    Name = _task.Name,
                    Estimation = TimeSpan.FromHours(System.Convert.ToDouble(_task.Estimation)),
                    Status = _task.Status,
                    CreationDate = _task.CreationDate,
                    StartAt = _task.StartAt,
                    EndAt = _task.EndAt,
                    ProjectId = _task.ProjectId
                };


                if (_task.Executors != null)
                {
                    var workerViewToBoConverter = new WorkerViewConverter();
                    var tempWorkers = new List<WorkerBO>();

                    foreach (var worker in _task.Executors)
                    {
                        var _worker = workerViewToBoConverter.Convert(worker);
                        _worker.Tasks = null;
                        tempWorkers.Add(_worker);

                    }
                    task.Workers = tempWorkers;
                }

                if (_task.Project != null)
                {
                    var projectViewToBoConverter = new ProjectViewConverter();
                    var _project = projectViewToBoConverter.Convert(_task.Project);

                    _project.Tasks = null;
                    task.Project = _project;
                }
                    
                return task;
            }

            return null;
        }
    }
}