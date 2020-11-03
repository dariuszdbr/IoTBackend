using System;
using System.IO;
using IoTBackend.Infrastructure.Core.Models;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Providers
{
    public interface IBlobPathProvider
    {
        string GetFilePath(string deviceId, DateTime dateTime, SensorType sensorType);
        string GetHistoricalFilePath(string deviceId, SensorType sensorType);
    }

    public class BlobPathProvider : IBlobPathProvider
    {
        public string GetFilePath(string deviceId, DateTime dateTime, SensorType sensorType)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(deviceId));

            return $"{Path.Combine(deviceId, sensorType.ToString().ToLower(), dateTime.ToString("yyyy-MM-dd"))}.csv";
        }

        public string GetHistoricalFilePath(string deviceId, SensorType sensorType)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(deviceId));

            return $"{Path.Combine(deviceId, sensorType.ToString().ToLower())}\\historical.zip";
        }
    }
}