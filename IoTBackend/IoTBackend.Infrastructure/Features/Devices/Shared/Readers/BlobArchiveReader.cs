using System;
using System.Linq;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Core.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;

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
            _blobClientProvider = blobClientProvider ?? throw new ArgumentNullException(nameof(blobClientProvider));
            _blobFilePathProvider = blobFilePathProvider ?? throw new ArgumentNullException(nameof(blobFilePathProvider));
            _zipArchiveProvider = zipArchiveProvider ?? throw new ArgumentNullException(nameof(zipArchiveProvider));
            _streamParser = streamParser ?? throw new ArgumentNullException(nameof(streamParser));
        }

        public async Task<BlobReaderResult> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            var filePath = _blobFilePathProvider.GetHistoricalFilePath(deviceId, parser.SensorType);
            var client = _blobClientProvider.GetClient(filePath);

            if (!await client.ExistsAsync()) return BlobReaderResult.CreatePathNotExistResult();

            using (var inputBlobStream = await client.OpenReadAsync())
            using (var inputArchiveStream = _zipArchiveProvider.GetZipArchive(inputBlobStream))
            {
                var entry = inputArchiveStream.Entries.FirstOrDefault(x => x.FullName == $"{dateTime:yyyy-MM-dd}.csv");
                if (entry == null) return BlobReaderResult.CreatePathNotExistResult();

                using (var entryArchiveStream = entry.Open())
                {
                    return BlobReaderResult.CreateResult(await _streamParser.ParseStreamAsync(parser, entryArchiveStream));
                }
            }
        }
    }
}