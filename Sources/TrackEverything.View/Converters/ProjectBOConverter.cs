using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Converters
{
    /// <summary>
    /// Converter that converts project business object
    /// to the project view Model
    /// </summary>
    public class ProjectBOConverter : IConverter<ProjectBO, ProjectViewModel>
    {
        public  ProjectViewModel Convert(ProjectBO projectBo)
        {
            if (projectBo != null)
            {
                var project = new ProjectViewModel
                {
                    Id = projectBo.Id,
                    Name = projectBo.Name,
                    Shortname = projectBo.Shortname,
                    Description = projectBo.Description,
                    Status = projectBo.Status,
                    CreationDate = projectBo.CreationDate,
                    Tasks = null
                };

                if (projectBo.Tasks != null)
                {
                    var taskBoToViewConverter = new TaskBOConverter();
                    var tempTasks = new List<TaskViewModel>();

                    foreach (var taskBo in projectBo.Tasks)
                    {
                        var task = taskBoToViewConverter.Convert(taskBo);
                        task.Project = null;
                        task.Executors = null;
                        tempTasks.Add(task);
                    }
                    project.Tasks = tempTasks;
                }

                return project;
            }

            return null;
        }
    }
}