using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Features.Devices.Parsers;

namespace IoTBackend.Infrastructure.Features.Devices.Interfaces
{
    public interface IBlobReader
    {
        Task<IEnumerable<ISensorDataPoint>> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser);
    }
}