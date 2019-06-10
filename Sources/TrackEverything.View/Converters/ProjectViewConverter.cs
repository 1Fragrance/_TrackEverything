using System;
using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Converters
{
    /// <summary>
    /// Converter that converts project view model
    /// to the project business object
    /// </summary>
    public class ProjectViewConverter : IConverter<ProjectViewModel, ProjectBO>
    {
        public ProjectBO Convert(ProjectViewModel _project)
        {
            if (_project != null)
            {
                var project = new ProjectBO
                {
                    Id = _project.Id,
                    Name = _project.Name,
                    Shortname = _project.Shortname,
                    Description = _project.Description,
                    Status = _project.Status,
                    CreationDate = _project.CreationDate,
                    Tasks = null
                };

                if (_project.Tasks != null)
                {
                    var taskViewToBoConverter = new TaskViewConverter();
                    var tempTasks = new List<TaskBO>();

                    foreach (var task in _project.Tasks)
                    {
                        var _task = taskViewToBoConverter.Convert(task);
                        _task.Project = null;
                        _task.Workers = null;
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