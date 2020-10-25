using System;
using IoTBackend.Infrastructure.Features.Devices.Interfaces;

namespace IoTBackend.Infrastructure.Features.Devices.Models
{
    public class TemperatureSensorDataPoint : ISensorDataPoint
    {
        public SensorType SensorType { get; } = SensorType.Temperature;
        public DateTime Date { get; }
        public double Value { get; }

        public TemperatureSensorDataPoint(DateTime dateTime, double value)
        {
            Date = dateTime;
            Value = value;
        }
    }
}