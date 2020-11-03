using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Readers
{
    public interface IBlobReader
    {
        Task<BlobReaderResult> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser);
    }
}