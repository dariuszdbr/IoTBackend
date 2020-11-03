using System;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;

namespace IoTBackend.Infrastructure.Core.Converters
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