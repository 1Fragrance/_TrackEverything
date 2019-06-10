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
    /// Controller that implements task entity API
    /// </summary>
    [Route("tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IService<TaskBO> taskService;
        private readonly IConverter<TaskBO, TaskViewModel> taskBoToTaskVmConverter;
        private readonly IConverter<TaskViewModel, TaskBO> taskVmToTaskBoConverter;

        public TaskController(IService<TaskBO> service, ILogger log,
            IConverter<TaskBO, TaskViewModel> taskBoToTaskVmConverter,
            IConverter<TaskViewModel, TaskBO> taskVmToTaskBoConverter)
        {
            taskService = service;
            logger = log;

            this.taskVmToTaskBoConverter = taskVmToTaskBoConverter;
            this.taskBoToTaskVmConverter = taskBoToTaskVmConverter;
        }

        // GET: tasks
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var tasks = taskService.GetAll();
                var tempTasks = new List<TaskViewModel>();
                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        var convertedTask = taskBoToTaskVmConverter.Convert(task);
                        tempTasks.Add(convertedTask);
                    }

                    return Ok(tempTasks);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogCritical("VIEW: Error in uploading all tasks ");
                logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                return StatusCode(500, ex);
            }
        }

        // GET: tasks/5
        [HttpGet("{id}", Name = "GetTask")]
        public IActionResult Get(int id)
        {
            if (id > 0)
            {
                try
                {
                    var taskBO = taskService.Get(id);
                    if (taskBO != null)
                    {
                        var convertedTask = taskBoToTaskVmConverter.Convert(taskBO);
                        return Ok(convertedTask);
                    }
                    return NoContent();

                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in uploading task #{id} ");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't get task with id < 1");
        }

        // GET: tasks/5/workers
        [HttpGet("{id}/workers")]
        public IActionResult GetWorkers(int id)
        {
            if (id > 0)
            {
                try
                {
                    var taskBO = taskService.Get(id);
                    if (taskBO != null)
                    {
                        var task = taskBoToTaskVmConverter.Convert(taskBO);
                        return Ok(task.Executors);
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in getting workers of task #{id} ");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't get task workers with id < 1");
        }

        // GET: tasks/5/project
        [HttpGet("{id}/project")]
        public IActionResult GetProject(int id)
        {
            if (id > 0)
            {
                try
                {
                    var taskBO = taskService.Get(id);
                    if (taskBO != null)
                    {
                        var task = taskBoToTaskVmConverter.Convert(taskBO);
                        return Ok(task.Project);
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in getting project of task #{id} ");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't get task project with id < 1");
        }

        // POST: tasks
        [HttpPost]
        public IActionResult Post([FromBody] TaskViewModel task)
        {
            if (task != null)
            {
                try
                {
                    var convertedTask = taskVmToTaskBoConverter.Convert(task);
                    var id = taskService.Add(convertedTask);
                    return CreatedAtRoute("GetTask", new {id}, task);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in adding {task.Name} task");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            logger.LogWarning("VIEW: Can't add empty task");
            return BadRequest("Can't add empty task");
        }

        // PUT: tasks/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TaskViewModel task)
        {
            if (task != null && id > 0)
            {
                try
                {
                    task.Id = id;
                    var convertedTask = taskVmToTaskBoConverter.Convert(task);
                    taskService.Update(convertedTask, id);

                    HttpContext.Response.Headers["Location"] = id.ToString();
                    return Ok(task);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in updating task #{id}");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't update task");
        }

        // DELETE: tasks/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                try
                {
                    taskService.Delete(id);
                    return NoContent(); 
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in deleting task #{id}");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }
            logger.LogWarning("VIEW: Can't delete task with id < 1");
            return BadRequest("Can't delete task with id < 1");
        }
    }
}