using System;
using IoTBackend.Infrastructure.Interfaces;

namespace IoTBackend.Infrastructure.Models
{
    public class RainfallSensorDataPoint : ISensorDataPoint
    {
        public SensorType SensorType { get; } = SensorType.Rainfall;
        public DateTime DateTime { get; }
        public double Value { get; }

        public RainfallSensorDataPoint(DateTime dateTime, double value)
        {
            DateTime = dateTime;
            Value = value;
        }
    }
}
