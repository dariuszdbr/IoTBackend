using System;
using IoTBackend.Infrastructure.Models;

namespace IoTBackend.Infrastructure.Interfaces
{
    public interface ISensorDataPoint
    {
        SensorType SensorType { get; }
        DateTime DateTime { get; }
        double Value { get; }
    }
}