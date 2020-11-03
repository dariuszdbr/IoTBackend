using System.Net;
using IoTBackend.Infrastructure.Core.Exceptions;

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