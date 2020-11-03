using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Readers
{
    public class BlobReader : IBlobReader
    {
        private readonly IEnumerable<IBlobReader> _blobReaders;

        public BlobReader(IEnumerable<IBlobReader> blobReaders)
        {
            _blobReaders = blobReaders ?? throw new ArgumentNullException(nameof(blobReaders));
        }

        public async Task<BlobReaderResult> ReadAsync(string deviceId, DateTime dateTime, ISensorDataParser parser)
        {
            var result = BlobReaderResult.CreatePathNotExistResult();
            foreach (var reader in _blobReaders)
            {
                result = await reader.ReadAsync(deviceId, dateTime, parser);

                if (result.PathExist) break;
                // There is a duplicate file 2019-01-10.csv in the historical.zip and outside.
                // If that's not a bug, should the results be merged?
            }

            return result;
        }
    }
}