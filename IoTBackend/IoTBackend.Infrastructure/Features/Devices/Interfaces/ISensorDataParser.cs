using IoTBackend.Infrastructure.Features.Devices.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Interfaces
{
    public interface ISensorDataParser
    {
        SensorType SensorType { get; }
        ISensorDataPoint Parse(string line);
    }
}