using System;
using System.Globalization;
using IoTBackend.Infrastructure.Interfaces;
using IoTBackend.Infrastructure.Models;

namespace IoTBackend.Infrastructure.Parsers
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