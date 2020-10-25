using System;
using IoTBackend.Core.Interfaces;

namespace IoTBackend.Core.Models
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
