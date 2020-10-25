using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IoTBackend.Core.Interfaces;

namespace IoTBackend.Infrastructure
{
    public interface IBlobReader
    {
        Task<List<ISensorDataPoint>> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser);
    }
}