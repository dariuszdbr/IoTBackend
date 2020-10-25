using System;
using IoTBackend.Infrastructure.Features.Devices.Exceptions;
using IoTBackend.Infrastructure.Features.Devices.Models;

namespace IoTBackend.Infrastructure.Shared.Converters
{
    public interface ISensorTypeConverter
    {
        SensorType Convert(string sensorType);
    }
 
    public class SensorTypeConverter : ISensorTypeConverter
    {
        public SensorType Convert(string sensorType)
        {
            if (Enum.TryParse<SensorType>(sensorType, true, out var type))
            {
                return type;
            }

            throw new NotSupportedSensorTypeException(sensorType);
        }
    }
}