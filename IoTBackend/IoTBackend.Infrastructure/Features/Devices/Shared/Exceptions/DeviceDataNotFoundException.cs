using System.Net;
using IoTBackend.Infrastructure.Shared.Exceptions;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions
{
    public class DeviceDataNotFoundException : DomainException
    {
        public DeviceDataNotFoundException() 
            : base("Data not found.", (int)HttpStatusCode.NotFound)
        {
        }
    }
}