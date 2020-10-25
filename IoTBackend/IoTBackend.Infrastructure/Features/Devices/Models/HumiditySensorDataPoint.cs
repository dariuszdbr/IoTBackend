using System;
using IoTBackend.Infrastructure.Features.Devices.Interfaces;

namespace IoTBackend.Infrastructure.Features.Devices.Models
{
    public class HumiditySensorDataPoint : ISensorDataPoint
    {
        public SensorType SensorType { get; } = SensorType.Humidity;
        public DateTime Date { get; }
        public double Value { get; }

        public HumiditySensorDataPoint(DateTime dateTime, double value)
        {
            Date = dateTime;
            Value = value;
        }
    }
}