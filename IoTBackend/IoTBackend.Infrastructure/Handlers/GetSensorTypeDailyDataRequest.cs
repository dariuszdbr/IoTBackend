using System;

namespace IoTBackend.Infrastructure.Handlers
{
    public class GetSensorTypeDailyDataRequest
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
        public string SensorType { get; set; }
    }
}