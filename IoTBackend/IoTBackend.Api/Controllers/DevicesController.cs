using System;
using System.Threading.Tasks;
using IoTBackend.Infrastructure;
using IoTBackend.Infrastructure.Features.Devices.Handlers.GetSensorTypeDailyData;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IoTBackend.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DevicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{deviceId}/data/{date}/{sensorType}")]
        public async Task<IActionResult> GetSensorTypeDailyData([FromRoute] GetSensorTypeDailyDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SensorType))
                return BadRequest();

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                return BadRequest();

            try
            {
                return Ok(await _mediator.Send(request));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "error occurred" });
            }
        }

        [HttpGet]
        [Route("{deviceId}/data/{date}")]
        public IActionResult GetDailyData(string deviceId, DateTime date)
        {
            return Ok();
        }
    }
}