using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Shared.Providers;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Readers
{
    public class BlobArchiveReader : IBlobReader
    {
        private readonly IBlobClientProvider _blobClientProvider;
        private readonly IBlobPathProvider _blobFilePathProvider;
        private readonly IStreamParser _streamParser;
        private readonly IZipArchiveProvider _zipArchiveProvider;

        public BlobArchiveReader(IBlobClientProvider blobClientProvider, IBlobPathProvider blobFilePathProvider, IZipArchiveProvider zipArchiveProvider, IStreamParser streamParser)
        {
            _blobClientProvider = blobClientProvider;
            _blobFilePathProvider = blobFilePathProvider;
            _zipArchiveProvider = zipArchiveProvider;
            _streamParser = streamParser;
        }

        public async Task<List<SensorDailyDataPoint>> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser)
        {
            var filePath = _blobFilePathProvider.GetHistoricalFilePath(deviceId, parser.SensorType);
            var client = _blobClientProvider.GetClient(filePath);

            if (!await client.ExistsAsync()) return new List<SensorDailyDataPoint>();

            using (var inputBlobStream = await client.OpenReadAsync())
            using (var inputArchiveStream = _zipArchiveProvider.GetZipArchive(inputBlobStream))
            {
                var entry = inputArchiveStream.Entries.FirstOrDefault(x => x.FullName == $"{dateTime:yyyy-MM-dd}.csv");
                if (entry == null) return new List<SensorDailyDataPoint>();

                using (var entryArchiveStream = entry.Open())
                {
                    return await _streamParser.ParseStream(parser, entryArchiveStream);
                }
            }
        }
    }
}