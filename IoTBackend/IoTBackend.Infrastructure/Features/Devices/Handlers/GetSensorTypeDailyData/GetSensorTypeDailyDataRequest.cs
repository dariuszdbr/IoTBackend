using System;
using MediatR;

namespace IoTBackend.Infrastructure.Features.Devices.Handlers.GetSensorTypeDailyData
{
    public class GetSensorTypeDailyDataRequest : IRequest<GetSensorTypeDailyDataRequestResponse>
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
        public string SensorType { get; set; }
    }
}