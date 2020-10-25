using System;
using IoTBackend.Infrastructure.Models;

namespace IoTBackend.Infrastructure.Converters
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

            throw new Exception("not supported type");
        }
    }
}