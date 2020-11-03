using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Parsers
{
    public interface ISensorDataParser
    {
        SensorType SensorType { get; }
        SensorDailyDataPoint Parse(string line);
    }
}