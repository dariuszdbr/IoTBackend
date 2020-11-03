using System;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Parsers
{
    public class TemperatureSensorDailyDataPointParser : ISensorDataParser
    {
        public SensorType SensorType { get; } = SensorType.Temperature;

        public SensorDailyDataPoint Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(line));

            var parts = line.Split(';');
            return new SensorDailyDataPoint(DateTime.Parse(parts[0]), double.Parse(parts[1]));
        }
    }
}