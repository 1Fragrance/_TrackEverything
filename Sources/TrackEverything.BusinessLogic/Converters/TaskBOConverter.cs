using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.Converters
{
    /// <summary>
    /// Converter that converts task business object
    /// to the storage task entity
    /// </summary>
    public class TaskBOConverter : IConverter<TaskBO, Task>
    {
        public Task Convert(TaskBO _task)
        {
            if (_task != null)
            {
                var task = new Task
                {
                    Id = _task.Id,
                    Name = _task.Name,
                    Estimation = _task.Estimation.Ticks,
                    Status = _task.Status,
                    CreationDate = _task.CreationDate,
                    StartAt = _task.StartAt,
                    EndAt = _task.EndAt,
                    ProjectId = _task.ProjectId
                };

                if (_task.Workers != null)
                {
                    var workerBoToDalConverter = new WorkerBOConverter();
                    var tempWorkers = new List<Worker>();

                    foreach (var worker in _task.Workers)
                    {
                        var _worker = workerBoToDalConverter.Convert(worker);
                        _worker.Tasks = null;
                        tempWorkers.Add(_worker);
                    }

                    task.Executors = tempWorkers;
                }

                if (_task.Project != null)
                {
                    var projectBoToDalConverter = new ProjectBOConverter();

                    var _project = projectBoToDalConverter.Convert(_task.Project);
                    _project.Tasks = null;
                    task.Project = _project;
                }

                return task;
            }

            return null;
        }
    }
}