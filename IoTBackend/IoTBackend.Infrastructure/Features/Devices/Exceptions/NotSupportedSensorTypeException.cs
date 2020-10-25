using System.Net;
using IoTBackend.Infrastructure.Features.Devices.Models;
using IoTBackend.Infrastructure.Shared.Exceptions;

namespace IoTBackend.Infrastructure.Features.Devices.Exceptions
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