using System;
using IoTBackend.Infrastructure.Interfaces;

namespace IoTBackend.Infrastructure.Models
{
    public class HumiditySensorDataPoint : ISensorDataPoint
    {
        public SensorType SensorType { get; } = SensorType.Humidity;
        public DateTime DateTime { get; }
        public double Value { get; }

        public HumiditySensorDataPoint(DateTime dateTime, double value)
        {
            DateTime = dateTime;
            Value = value;
        }
    }
}