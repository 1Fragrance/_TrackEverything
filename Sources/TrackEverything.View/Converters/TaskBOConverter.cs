using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Converters
{
    /// <summary>
    ///     Converter that converts task business model
    ///     to the task view model
    /// </summary>
    public class TaskBOConverter : IConverter<TaskBO, TaskViewModel>
    {
        public TaskViewModel Convert(TaskBO _task)
        {
            if (_task != null)
            {
                var task = new TaskViewModel
                {
                    Id = _task.Id,
                    Name = _task.Name,
                    Estimation = _task.Estimation.TotalHours.ToString(),
                    Status = _task.Status,
                    CreationDate = _task.CreationDate,
                    StartAt = _task.StartAt,
                    EndAt = _task.EndAt,
                    ProjectId = _task.ProjectId,
                    Executors = null
                };
 
                if (_task.Workers != null)
                {
                    var workerBoToViewConverter = new WorkerBOConverter();
                    var tempWorkers = new List<WorkerViewModel>();

                    foreach (var worker in _task.Workers)
                    {
                        var _worker = workerBoToViewConverter.Convert(worker);
                        _worker.Tasks = null;
                        tempWorkers.Add(_worker);
                    }

                    task.Executors = tempWorkers;
                }

                if (_task.Project != null)
                {
                    var projectBoToViewConverter = new ProjectBOConverter();

                    var _project = projectBoToViewConverter.Convert(_task.Project);
                    _project.Tasks = null;
                    task.Project = _project;
                }

                return task;
            }

            return null;
        }
    }
}