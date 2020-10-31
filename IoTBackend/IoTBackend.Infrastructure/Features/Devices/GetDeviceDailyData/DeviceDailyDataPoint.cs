using System;

namespace IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData
{
    public class DeviceDailyDataPoint
    {
        public DateTime Date { get; }
        public double Temperature { get; }
        public double Humidity { get; }
        public double Rainfall { get; }

        public DeviceDailyDataPoint(DateTime dateTime, double temperature, double humidity, double rainfall)
        {
            Date = dateTime;
            Temperature = temperature;
            Humidity = humidity;
            Rainfall = rainfall;
        }
    }
}