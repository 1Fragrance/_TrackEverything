using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrackEverything.BusinessLogic.BusinessObjects;
using TrackEverything.BusinessLogic.Interfaces;
using TrackEverything.View.Converters;
using TrackEverything.View.ViewModels;

namespace TrackEverything.View.Controllers
{
    /// <summary>
    /// Controller that implements project entity API
    /// </summary>
    [Route("projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IService<ProjectBO> projectService;
        private readonly IConverter<ProjectBO, ProjectViewModel> projectBoToProjectVmConverter;
        private readonly IConverter<ProjectViewModel, ProjectBO> projectVmToProjectBoConverter;

        public ProjectController(IService<ProjectBO> service, ILogger log,
            IConverter<ProjectBO, ProjectViewModel> projectBoToProjectVmConverter,
            IConverter<ProjectViewModel, ProjectBO> projectVmToProjectBoConverter)
        {
            projectService = service;
            logger = log;

            this.projectBoToProjectVmConverter = projectBoToProjectVmConverter;
            this.projectVmToProjectBoConverter = projectVmToProjectBoConverter;
        }

        // GET: projects
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var projects = projectService.GetAll();
                var tempProjects = new List<ProjectViewModel>();
                if (projects != null)
                {
                    foreach (var project in projects)
                    {
                        var convertedProject = projectBoToProjectVmConverter.Convert(project);
                        tempProjects.Add(convertedProject);
                    }

                    return Ok(tempProjects);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogCritical("VIEW: Error in uploading all projects ");
                logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                return StatusCode(500, ex);
            }
        }

        // GET: projects/5
        [HttpGet("{id}", Name = "GetProject")]
        public IActionResult Get(int id)
        {
            if (id > 0)
            {
                try
                {
                    var projectBO = projectService.Get(id);
                    if (projectBO != null)
                    {
                        var convertedObject = projectBoToProjectVmConverter.Convert(projectBO);
                        return Ok(convertedObject);
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in uploading project #{id} ");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't get project with id < 1");
        }


        // GET: projects/5/tasks
        [HttpGet("{id}/tasks")]
        public IActionResult GetTasks(int id)
        {
            if (id > 0)
            {
                try
                {

                    var projectBO = projectService.Get(id);
                    if (projectBO != null)
                    {
                        var project = projectBoToProjectVmConverter.Convert(projectBO);
                        return Ok(project.Tasks);
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in getting tasks of project #{id} ");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't get project task with id < 1");
        }

        // POST: projects
        [HttpPost]
        public IActionResult Post([FromBody] ProjectViewModel project)
        {
            if (project != null)
            {
                try
                {
                    var convertedProject = projectVmToProjectBoConverter.Convert(project);
                    var id = projectService.Add(convertedProject);
                    return CreatedAtRoute("GetProject", new {id}, project);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in adding project {project.Name}");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);

                    return StatusCode(500, ex);
                }
            }

            logger.LogWarning("VIEW: Can't add empty project");
            return BadRequest("Can't add empty project");
        }

        // PUT: projects/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ProjectViewModel project)
        {
            if (project != null && id > 0)
            {
                try
                {
                    project.Id = id;
                    var convertedProject = projectVmToProjectBoConverter.Convert(project);
                    projectService.Update(convertedProject, id);

                    HttpContext.Response.Headers["Location"] = id.ToString();
                    return Ok(project);

                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in updating project #{id}");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't update project");
        }

        // DELETE: projects/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                try
                {
                    projectService.Delete(id);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in deleting project #{id}");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            logger.LogWarning("VIEW: Can't delete project with id < 1");
            return BadRequest("Can't delete project with id < 1");
        }
    }
}