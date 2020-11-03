using System;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Core.Exceptions;
using IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
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
        public async Task<IActionResult> GetSensorTypeDailyData([FromRoute] GetSensorDailyDataRequest request)
        {
            if (request == null)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(request.SensorType))
                return BadRequest();

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                return BadRequest();

            try
            {
                return Ok(await _mediator.Send(request));
            }
            catch (DomainException e)
            {
                return StatusCode(e.StatusCode, new { message = e.Message } );
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "error occurred" });
            }
        }

        [HttpGet]
        [Route("{deviceId}/data/{date}")]
        public async Task<IActionResult> GetDeviceDailyData([FromRoute] GetDeviceDailyDataRequest request)
        {
            if (request == null)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                return BadRequest();

            try
            {
                return Ok(await _mediator.Send(request));
            }
            catch (DomainException e)
            {
                return StatusCode(e.StatusCode, new { message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "error occurred" });
            }
        }
    }
}