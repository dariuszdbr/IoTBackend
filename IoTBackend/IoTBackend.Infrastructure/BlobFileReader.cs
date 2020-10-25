using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IoTBackend.Core.Interfaces;

namespace IoTBackend.Infrastructure
{
    public class BlobFileReader : IBlobReader
    {
        private readonly IBlobClientProvider _blobClientProvider;
        private readonly IBlobPathProvider _blobFilePathProvider;
        private readonly IStreamParser _streamParser;

        public BlobFileReader(IBlobClientProvider blobClientProvider, IBlobPathProvider blobFilePathProvider, IStreamParser streamParser)
        {
            _blobClientProvider = blobClientProvider;
            _streamParser = streamParser;
            _blobFilePathProvider = blobFilePathProvider;
        }

        public async Task<List<ISensorDataPoint>> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser)
        {
            var filePath = _blobFilePathProvider.GetFilePath(deviceId, dateTime, parser.SensorType);
            var client = _blobClientProvider.GetClient(filePath);

            if (!await client.ExistsAsync()) return null;

            using (var stream = await client.OpenReadAsync())
            {
                return await _streamParser.ParseStream(parser, stream);
            }
        }
    }
}