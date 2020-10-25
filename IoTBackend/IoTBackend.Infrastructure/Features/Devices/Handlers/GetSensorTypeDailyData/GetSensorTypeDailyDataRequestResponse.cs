using System.Collections.Generic;
using IoTBackend.Infrastructure.Features.Devices.Dtos;

namespace IoTBackend.Infrastructure.Features.Devices.Handlers.GetSensorTypeDailyData
{
    public class GetSensorTypeDailyDataRequestResponse
    {
        public IEnumerable<SensorDataPointDto> Data { get; set; }
    }
}