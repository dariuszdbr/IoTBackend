using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Core.Providers;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Parsers
{
    public interface IStreamParser
    {
        Task<List<SensorDailyDataPoint>> ParseStreamAsync(ISensorDataParser parser, Stream stream);
    }

    public class StreamParser : IStreamParser
    {
        private readonly IStreamReaderProvider _streamReaderProvider;

        public StreamParser(IStreamReaderProvider streamReaderProvider)
        {
            _streamReaderProvider = streamReaderProvider;
        }

        public async Task<List<SensorDailyDataPoint>> ParseStreamAsync(ISensorDataParser parser, Stream stream)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            List<SensorDailyDataPoint> sensorsData = new List<SensorDailyDataPoint>();
            using (var reader = _streamReaderProvider.GetReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var data = parser.Parse(line);

                    sensorsData.Add(data);
                }
            }

            return sensorsData;
        }
    }
}