using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Shared.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Parsers
{
    public interface ISensorDataParser
    {
        SensorType SensorType { get; }
        SensorDailyDataPoint Parse(string line);
    }
}