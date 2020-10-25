using System;
using System.Globalization;
using IoTBackend.Core.Interfaces;
using IoTBackend.Core.Models;

namespace IoTBackend.Infrastructure
{
    public class RainfallDataParser : ISensorDataParser
    {
        public SensorType SensorType { get; } = SensorType.Rainfall;

        public ISensorDataPoint Parse(string line)
        {
            var parts = line.Split(';');
            return new RainfallSensorDataPoint(DateTime.Parse(parts[0]), double.Parse(parts[1], new CultureInfo("pl-PL")));
        }
    }
}