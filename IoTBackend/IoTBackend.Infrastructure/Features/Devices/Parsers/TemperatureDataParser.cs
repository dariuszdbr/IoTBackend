using System;
using System.Globalization;
using IoTBackend.Infrastructure.Features.Devices.Interfaces;
using IoTBackend.Infrastructure.Features.Devices.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Parsers
{
    public class TemperatureDataParser : ISensorDataParser
    {
        public SensorType SensorType { get; } = SensorType.Temperature;

        public ISensorDataPoint Parse(string line)
        {
            var parts = line.Split(';');
            return new TemperatureSensorDataPoint(DateTime.Parse(parts[0]), double.Parse(parts[1], new CultureInfo("pl-PL")));
        }
    }
}