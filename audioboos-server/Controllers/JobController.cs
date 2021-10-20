using System;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AudioBoos.Server.Controllers {
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class JobController : ControllerBase {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<JobController> _logger;

        public JobController(ISchedulerFactory schedulerFactory, ILogger<JobController> logger) {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<ActionResult<JobStartDto>> StartJob([FromQuery] string name,
            CancellationToken cancellationToken) {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            try {
                await scheduler.TriggerJob(new JobKey(name, "DEFAULT"), cancellationToken);
                return Ok();
            } catch (Exception e) {
                _logger.LogError("Error starting job {Message}", e.Message);
            }
            return StatusCode(500);
        }
    }
}
