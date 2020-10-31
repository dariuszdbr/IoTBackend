using System;
using MediatR;

namespace IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData
{
    public class GetSensorDailyDataRequest : IRequest<GetSensorDailyDataResponse>
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
        public string SensorType { get; set; }
    }
}