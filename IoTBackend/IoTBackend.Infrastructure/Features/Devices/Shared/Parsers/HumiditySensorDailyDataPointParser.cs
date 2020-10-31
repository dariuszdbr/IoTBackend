using System;
using System.Globalization;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Shared.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Parsers
{
    public class HumiditySensorDailyDataPointParser : ISensorDataParser
    {
        public SensorType SensorType { get; } = SensorType.Humidity;
        public virtual SensorDailyDataPoint Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) throw new ArgumentException("Value cannot be null, empty or whitespace.", nameof(line));

            var parts = line.Split(';');
            return new SensorDailyDataPoint(DateTime.Parse(parts[0]), double.Parse(parts[1]));
        }
    }
}