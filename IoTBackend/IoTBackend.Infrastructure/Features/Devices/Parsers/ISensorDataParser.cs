using IoTBackend.Infrastructure.Features.Devices.Interfaces;
using IoTBackend.Infrastructure.Features.Devices.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Parsers
{
    public interface ISensorDataParser
    {
        SensorType SensorType { get; }
        ISensorDataPoint Parse(string line);
    }
}