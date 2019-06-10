using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Converters
{
    /// <summary>
    /// Converter that converts worker business object
    /// to the worker view model
    /// </summary>
    public class WorkerBOConverter : IConverter<WorkerBO, WorkerViewModel>
    {
        public WorkerViewModel Convert(WorkerBO _worker)
        {
            if (_worker != null)
            {
                var worker = new WorkerViewModel
                {
                    Id = _worker.Id,
                    Name = _worker.Name,
                    Surname = _worker.Surname,
                    MiddleName = _worker.MiddleName,
                    Position = _worker.Position,
                    Tasks = null
                };

                
                if (_worker.Tasks != null)
                {
                    var taskBoToViewConverter = new TaskBOConverter();
                    var tempTasks = new List<TaskViewModel>();

                    foreach (var task in _worker.Tasks)
                    {
                        var _task = taskBoToViewConverter.Convert(task);
                        _task.Project = null;
                        _task.Executors = null;
                        tempTasks.Add(_task);
                    }
                    worker.Tasks = tempTasks;
                }

                return worker;
            }

            return null;
        }
    }
}