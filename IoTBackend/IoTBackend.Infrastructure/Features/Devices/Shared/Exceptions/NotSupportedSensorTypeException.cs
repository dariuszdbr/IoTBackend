using System.Net;
using IoTBackend.Infrastructure.Core.Exceptions;
using IoTBackend.Infrastructure.Core.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions
{
    public class NotSupportedSensorTypeException : DomainException
    {
        public NotSupportedSensorTypeException(SensorType type) :base($"Sensor type: {type} - is not supported.", (int)HttpStatusCode.BadRequest)
        {
            
        }

        public NotSupportedSensorTypeException(string sensorType) : base($"Sensor type: {sensorType} - is not supported.", (int)HttpStatusCode.BadRequest)
        {

        }

    }
}