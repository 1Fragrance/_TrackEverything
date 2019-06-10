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
    /// Controller that implements worker entity API
    /// </summary>
   
    [Route("workers")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IService<WorkerBO> workerService;
        private readonly IConverter<WorkerBO, WorkerViewModel> workerBoToWorkerVmConverter;
        private readonly IConverter<WorkerViewModel, WorkerBO> workerVmToWorkerBoConverter;

        public WorkerController(IService<WorkerBO> service, ILogger log,
            IConverter<WorkerBO, WorkerViewModel> workerBoToWorkerVmConverter,
            IConverter<WorkerViewModel, WorkerBO> workerVmToWorkerBoConverter)
        {
            workerService = service;
            logger = log;

            this.workerBoToWorkerVmConverter = workerBoToWorkerVmConverter;
            this.workerVmToWorkerBoConverter = workerVmToWorkerBoConverter;
        }

        // GET: workers
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var workers = workerService.GetAll();
                var tempWorkers = new List<WorkerViewModel>();
                if (workers != null)
                {
                    foreach (var task in workers)
                    {
                        var convertedTask = workerBoToWorkerVmConverter.Convert(task);
                        tempWorkers.Add(convertedTask);
                    }

                    return Ok(tempWorkers);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogCritical("VIEW: Error in uploading all workers ");
                logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                return StatusCode(500, ex);
            }
        }

        // GET: workers/5
        [HttpGet("{id}", Name = "GetWorker")]
        public IActionResult Get(int id)
        {
            if (id > 0)
            {
                try
                {
                    var workerBO = workerService.Get(id);
                    if (workerBO != null)
                    {
                        var convertedWorker = workerBoToWorkerVmConverter.Convert(workerBO);
                        return Ok(convertedWorker);
                    }

                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in uploading worker #{id} ");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't get worker with id < 1");
        }

        // GET: workers/5/tasks
        [HttpGet("{id}/tasks")]
        public IActionResult GetTasks(int id)
        {
            if (id > 0)
            {
                try
                {
                    var workerBO = workerService.Get(id);
                    if (workerBO != null)
                    {
                        var worker = workerBoToWorkerVmConverter.Convert(workerBO);
                        return Ok(worker.Tasks);
                    }

                    return NoContent();

                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in getting tasks of worker #{id} ");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't get tasks of the worker with id < 1");
        }

        // POST: workers
        [HttpPost]
        public IActionResult Post([FromBody] WorkerViewModel worker)
        {
            if (worker != null)
            {
                try
                {
                    var convertedWorker = workerVmToWorkerBoConverter.Convert(worker);
                    var id = workerService.Add(convertedWorker);
                    return CreatedAtRoute("GetWorker", new {id}, worker);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in adding {worker.Name} worker");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            logger.LogWarning("VIEW: Can't add empty worker");
            return BadRequest("Can't add empty worker");
        }

        // PUT: workers/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] WorkerViewModel worker)
        {
            if (worker != null && id > 0)
            {
                try
                {
                    worker.Id = id;
                    var convertedWorker = workerVmToWorkerBoConverter.Convert(worker);
                    workerService.Update(convertedWorker, id);

                    HttpContext.Response.Headers["Location"] = id.ToString();
                    return Ok(worker);
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in updating worker #{id}");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            return BadRequest("Can't update worker");
        }

        // DELETE: workers/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                try
                {
                    workerService.Delete(id);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogCritical($"VIEW: Error in deleting worker #{id}");
                    logger.LogTrace(ex.Message + "\n" + ex.StackTrace);
                    return StatusCode(500, ex);
                }
            }

            logger.LogWarning("VIEW: Can't delete worker with id < 1");
            return BadRequest("Cant delete worker with id < 1");
        }
    }
}