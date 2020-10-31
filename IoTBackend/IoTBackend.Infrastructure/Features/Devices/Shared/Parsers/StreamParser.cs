using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Shared.Providers;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Parsers
{
    public interface IStreamParser
    {
        Task<List<SensorDailyDataPoint>> ParseStream(ISensorDataParser parser, Stream stream);
    }

    public class StreamParser : IStreamParser
    {
        private readonly IStreamReaderProvider _streamReaderProvider;

        public StreamParser(IStreamReaderProvider streamReaderProvider)
        {
            _streamReaderProvider = streamReaderProvider;
        }

        public async Task<List<SensorDailyDataPoint>> ParseStream(ISensorDataParser parser, Stream stream)
        {
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