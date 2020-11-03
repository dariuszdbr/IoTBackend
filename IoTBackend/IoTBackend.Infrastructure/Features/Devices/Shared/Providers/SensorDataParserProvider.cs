using System.Collections.Generic;
using System.Linq;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Providers
{
    public interface ISensorDataParserProvider
    {
        ISensorDataParser GetParser(SensorType sensorType);
    }

    public class SensorDataParserProvider : ISensorDataParserProvider
    {
        private readonly Dictionary<SensorType, ISensorDataParser> _container;

        public SensorDataParserProvider(IEnumerable<ISensorDataParser> parsers)
        {
            _container = parsers.ToDictionary(parser => parser.SensorType, parser => parser);
        }

        public ISensorDataParser GetParser(SensorType sensorType)
        {
            if (_container.TryGetValue(sensorType, out var parser))
            {
                return parser;
            }

            throw new NotSupportedSensorTypeException(sensorType);
        }
    }
}