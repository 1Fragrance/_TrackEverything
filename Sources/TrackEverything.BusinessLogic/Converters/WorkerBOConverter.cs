using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.Converters
{
    /// <summary>
    /// Converter that converts worker business object
    /// to the storage worker entity
    /// </summary>
    public class WorkerBOConverter : IConverter<WorkerBO, Worker>
    {
        public Worker Convert(WorkerBO _worker)
        {
            if (_worker != null)
            {
                var worker = new Worker
                {
                    Id = _worker.Id,
                    Name = _worker.Name,
                    Surname = _worker.Surname,
                    MiddleName = _worker.MiddleName,
                    Position = _worker.Position
                };

                if (_worker.Tasks != null)
                {
                    var taskBoToDalConverter = new TaskBOConverter();
                    var tempTasks = new List<Task>();

                    foreach (var task in _worker.Tasks)
                    {
                        var _task = taskBoToDalConverter.Convert(task);
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