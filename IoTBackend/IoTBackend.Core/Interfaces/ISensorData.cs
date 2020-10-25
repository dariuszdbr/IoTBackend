using System;
using IoTBackend.Core.Models;

namespace IoTBackend.Core.Interfaces
{
    public interface ISensorDataPoint
    {
        SensorType SensorType { get; }
        DateTime DateTime { get; }
        double Value { get; }
    }
}