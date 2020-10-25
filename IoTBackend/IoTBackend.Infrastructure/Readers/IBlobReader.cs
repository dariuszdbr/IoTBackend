using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Interfaces;
using IoTBackend.Infrastructure.Parsers;

namespace IoTBackend.Infrastructure.Readers
{
    public interface IBlobReader
    {
        Task<List<ISensorDataPoint>> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser);
    }
}