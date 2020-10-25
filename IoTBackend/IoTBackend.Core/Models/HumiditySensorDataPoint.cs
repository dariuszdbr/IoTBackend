using System;
using IoTBackend.Core.Interfaces;

namespace IoTBackend.Core.Models
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