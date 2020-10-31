using System.Collections.Generic;

namespace IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData
{
    public class GetDeviceDailyDataResponse
    {
        public IEnumerable<DeviceDailyDataPoint> Data { get; set; }
    }
}