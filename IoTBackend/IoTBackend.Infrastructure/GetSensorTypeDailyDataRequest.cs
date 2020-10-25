using System;

namespace IoTBackend.Infrastructure
{
    public class GetSensorTypeDailyDataRequest
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
        public string SensorType { get; set; }
    }
}