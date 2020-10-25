using System;
using IoTBackend.Core.Interfaces;

namespace IoTBackend.Core.Models
{
    public class TemperatureSensorDataPoint : ISensorDataPoint
    {
        public SensorType SensorType { get; } = SensorType.Temperature;
        public DateTime DateTime { get; }
        public double Value { get; }

        public TemperatureSensorDataPoint(DateTime dateTime, double value)
        {
            DateTime = dateTime;
            Value = value;
        }
    }
}