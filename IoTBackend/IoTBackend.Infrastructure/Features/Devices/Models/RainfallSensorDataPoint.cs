using System;
using IoTBackend.Infrastructure.Features.Devices.Interfaces;

namespace IoTBackend.Infrastructure.Features.Devices.Models
{
    public class RainfallSensorDataPoint : ISensorDataPoint
    {
        public SensorType SensorType { get; } = SensorType.Rainfall;
        public DateTime Date { get; }
        public double Value { get; }

        public RainfallSensorDataPoint(DateTime dateTime, double value)
        {
            Date = dateTime;
            Value = value;
        }
    }
}
