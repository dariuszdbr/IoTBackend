using System;

namespace IoTBackend.Infrastructure.Features.Devices.Dtos
{
    public class SensorDataPointDto
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}