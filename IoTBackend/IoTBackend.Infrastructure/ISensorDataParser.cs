using IoTBackend.Core.Interfaces;
using IoTBackend.Core.Models;

namespace IoTBackend.Infrastructure
{
    public interface ISensorDataParser
    {
        SensorType SensorType { get; }
        ISensorDataPoint Parse(string line);
    }
}