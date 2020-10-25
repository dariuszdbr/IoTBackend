using IoTBackend.Infrastructure.Interfaces;
using IoTBackend.Infrastructure.Models;

namespace IoTBackend.Infrastructure.Parsers
{
    public interface ISensorDataParser
    {
        SensorType SensorType { get; }
        ISensorDataPoint Parse(string line);
    }
}