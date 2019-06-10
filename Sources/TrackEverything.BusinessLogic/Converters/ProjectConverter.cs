using System;
using System.Collections.Generic;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.Storage.Core.Entities;

namespace TrackEverything.BusinessLogic.Converters
{
    /// <summary>
    /// Converter that converts project storage entity
    /// to the project business object
    /// </summary>
    public class ProjectConverter : IConverter<Project, ProjectBO>
    {
        public ProjectBO Convert(Project _project)
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
                    CreationDate = _project.CreationDate
                };

                if (_project.Tasks != null)
                {
                    var tempTasks = new List<TaskBO>();
                    foreach (var task in _project.Tasks)
                        tempTasks.Add(new TaskBO
                        {
                            Id = task.Id,
                            Name = task.Name,
                            Estimation = TimeSpan.FromTicks(task.Estimation),
                            Status = task.Status,
                            CreationDate = task.CreationDate,
                            StartAt = task.StartAt,
                            EndAt = task.EndAt,
                            Workers = null,
                            ProjectId = _project.Id,
                            Project = project
                        });
                    project.Tasks = tempTasks;
                }
                if (_project.Tasks != null)
                {
                    var taskDalToBoConverter = new TaskConverter();
                    var tempTasks = new List<TaskBO>();

                    foreach (var task in _project.Tasks)
                    {
                        var _task = taskDalToBoConverter.Convert(task);
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