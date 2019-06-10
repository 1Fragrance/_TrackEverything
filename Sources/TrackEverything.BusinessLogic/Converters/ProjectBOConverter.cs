using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.Converters
{
    /// <summary>
    /// Converter that converts project business object
    /// to the storage project entity
    /// </summary>
    public class ProjectBOConverter : IConverter<ProjectBO, Project>
    {
        public Project Convert(ProjectBO _project)
        {
            if (_project != null)
            {
                var project = new Project
                {
                    Id = _project.Id,
                    Name = _project.Name,
                    Shortname = _project.Shortname,
                    Description = _project.Description,
                    Status = _project.Status,
                    CreationDate = _project.CreationDate
                };

                if (_project.Tasks != null)
                {
                    var taskBoToDalConverter = new TaskBOConverter();
                    var tempTasks = new List<Task>();

                    foreach (var task in _project.Tasks)
                    {
                        var _task = taskBoToDalConverter.Convert(task);
                        _task.Project = null;
                        _task.Executors = null;
                        tempTasks.Add(_task);
                    }
                    project.Tasks = tempTasks;
                }

                return project;
            }

            return null;
        }
    }
}