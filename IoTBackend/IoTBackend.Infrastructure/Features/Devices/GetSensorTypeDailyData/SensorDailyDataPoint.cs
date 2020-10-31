using System;

namespace IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData
{
    public class SensorDailyDataPoint
    {
        public DateTime Date { get; }
        public double Value { get; }

        public SensorDailyDataPoint(DateTime dateTime, double value)
        {
            Date = dateTime;
            Value = value;
        }
    }
}