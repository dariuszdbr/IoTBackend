using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Interfaces;
using IoTBackend.Infrastructure.Providers;

namespace IoTBackend.Infrastructure.Parsers
{
    public interface IStreamParser
    {
        Task<List<ISensorDataPoint>> ParseStream(ISensorDataParser parser, Stream stream);
    }

    public class StreamParser : IStreamParser
    {
        private readonly IStreamReaderProvider _streamReaderProvider;

        public StreamParser(IStreamReaderProvider streamReaderProvider)
        {
            _streamReaderProvider = streamReaderProvider;
        }

        public async Task<List<ISensorDataPoint>> ParseStream(ISensorDataParser parser, Stream stream)
        {
            List<ISensorDataPoint> sensorsData = new List<ISensorDataPoint>();
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