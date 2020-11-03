using System.Collections.Generic;

namespace IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData
{
    public class GetSensorDailyDataResponse
    {
        public IEnumerable<SensorDailyDataPoint> Data { get; set; }

        public GetSensorDailyDataResponse()
        {
            Data = new List<SensorDailyDataPoint>();
        }
    }
}