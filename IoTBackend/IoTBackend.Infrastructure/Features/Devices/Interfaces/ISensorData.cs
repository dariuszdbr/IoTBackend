using System;
using IoTBackend.Infrastructure.Features.Devices.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Interfaces
{
    public interface ISensorDataPoint
    {
        DateTime Date { get; }
        double Value { get; }
    }
}