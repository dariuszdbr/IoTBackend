using System;
using IoTBackend.Infrastructure.Interfaces;

namespace IoTBackend.Infrastructure.Models
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