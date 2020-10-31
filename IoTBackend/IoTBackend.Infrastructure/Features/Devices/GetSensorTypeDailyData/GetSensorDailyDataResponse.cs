using System.Collections.Generic;

namespace IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData
{
    public class GetSensorDailyDataResponse
    {
        public IEnumerable<SensorDailyDataPoint> Data { get; set; }
    }
}