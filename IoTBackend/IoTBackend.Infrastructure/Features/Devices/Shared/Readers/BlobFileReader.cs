using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Core.Providers;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Readers
{
    public class BlobFileReader : IBlobReader
    {
        private readonly IBlobClientProvider _blobClientProvider;
        private readonly IBlobPathProvider _blobFilePathProvider;
        private readonly IStreamParser _streamParser;

        public BlobFileReader(IBlobClientProvider blobClientProvider, IBlobPathProvider blobFilePathProvider, IStreamParser streamParser)
        {
            _blobClientProvider = blobClientProvider ?? throw new ArgumentNullException(nameof(blobClientProvider));
            _streamParser = streamParser ?? throw new ArgumentNullException(nameof(streamParser));
            _blobFilePathProvider = blobFilePathProvider ?? throw new ArgumentNullException(nameof(blobFilePathProvider));
        }

        public async Task<BlobReaderResult> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser)
        {
            var filePath = _blobFilePathProvider.GetFilePath(deviceId, dateTime, parser.SensorType);
            var client = _blobClientProvider.GetClient(filePath);

            if (!await client.ExistsAsync()) return BlobReaderResult.CreatePathNotExistResult();

            using var stream = await client.OpenReadAsync();
            return BlobReaderResult.CreateResult(await _streamParser.ParseStreamAsync(parser, stream));
        }
    }
}