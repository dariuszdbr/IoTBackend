using System;
using System.Globalization;
using IoTBackend.Core.Interfaces;
using IoTBackend.Core.Models;

namespace IoTBackend.Infrastructure
{
    public class HumidityDataParser : ISensorDataParser
    {
        public SensorType SensorType { get; } = SensorType.Humidity;

        public ISensorDataPoint Parse(string line)
        {
            var parts = line.Split(';');
            return new HumiditySensorDataPoint(DateTime.Parse(parts[0]), double.Parse(parts[1], new CultureInfo("pl-PL")));
        }
    }
}