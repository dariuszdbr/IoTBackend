using System;
using MediatR;

namespace IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData
{
    public class GetDeviceDailyDataRequest : IRequest<GetDeviceDailyDataResponse>
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
    }
}