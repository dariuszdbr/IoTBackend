﻿using System;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Shared.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Parsers
{
    public class TemperatureDataParser : ISensorDataParser
    {
        public SensorType SensorType { get; } = SensorType.Temperature;

        public virtual SensorDailyDataPoint Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                throw new ArgumentNullException(nameof(line), "Value cannot be null or whitespace.");

            var parts = line.Split(';');
            return new SensorDailyDataPoint(DateTime.Parse(parts[0]), double.Parse(parts[1]));
        }
    }
}