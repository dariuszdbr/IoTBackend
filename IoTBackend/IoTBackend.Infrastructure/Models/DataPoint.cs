using System;
using IoTBackend.Infrastructure.Interfaces;

namespace IoTBackend.Infrastructure.Models
{
    public class DataPoint
    {
        public DateTime DateTime { get; }
        public ISensorDataPoint Temperature { get; }
        public ISensorDataPoint Humidity { get; }
        public ISensorDataPoint Rainfall { get; }

        private DataPoint(DateTime dateTime, ISensorDataPoint temperature, ISensorDataPoint humidity, ISensorDataPoint rainfall)
        {
            DateTime = dateTime;
            Temperature = temperature;
            Humidity = humidity;
            Rainfall = rainfall;
        }

        public static DataPoint Create(
            DateTime dateTime,
            TemperatureSensorDataPoint temperature,
            HumiditySensorDataPoint humidity,
            RainfallSensorDataPoint rainfall)
        {
            // assert not null

            return new DataPoint(dateTime, temperature, humidity, rainfall);
        }
    }
}