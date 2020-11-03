using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using NUnit.Framework;
using System;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Providers
{
    [TestFixture]
    public class BlobPathProviderTests
    {

        [TestCase(SensorType.Rainfall)]
        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        public void GetFilePath_WhenProvidedNullDeviceId_ThenThrowArgumentException(SensorType @case)
        {
            // Arrange
            var unitUnderTest = new BlobPathProvider();
            string deviceId = null;
            var dateTime = new DateTime(2020, 10, 31, 17, 20, 0);
            var sensorType = @case;

            // Act
            Action result = () => unitUnderTest.GetFilePath(deviceId, dateTime, sensorType);

            // Assert
            result.Should().ThrowExactly<ArgumentException>();
        }

        [TestCase(SensorType.Rainfall, @"deviceId\rainfall\2020-10-31.csv")]
        [TestCase(SensorType.Humidity, @"deviceId\humidity\2020-10-31.csv")]
        [TestCase(SensorType.Temperature, @"deviceId\temperature\2020-10-31.csv")]
        public void GetFilePath_WhenProvidedValidParams_ThenReturnCombinedPathWithShortDate(SensorType @case, string sensorTypeExpectedPath)
        {
            // Arrange
            var unitUnderTest = new BlobPathProvider();

            string deviceId = "deviceId";
            var dateTime = new DateTime(2020, 10, 31, 17, 20, 0);
            var sensorType = @case;

            // Act
            var result = unitUnderTest.GetFilePath(
                deviceId,
                dateTime,
                sensorType);

            // Assert
            result.Should().BeEquivalentTo(sensorTypeExpectedPath);
        }

        [TestCase(SensorType.Rainfall)]
        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        public void GetHistoricalFilePath_WhenProvidedNullDeviceId_ThenThrowArgumentException(SensorType @case)
        {
            // Arrange
            var unitUnderTest = new BlobPathProvider();
            string deviceId = null;
            var sensorType = @case;

            // Act
            Action result = () => unitUnderTest.GetHistoricalFilePath(deviceId, sensorType);

            // Assert
            result.Should().ThrowExactly<ArgumentException>();
        }

        [TestCase(SensorType.Rainfall, @"deviceId\rainfall\historical.zip")]
        [TestCase(SensorType.Humidity, @"deviceId\humidity\historical.zip")]
        [TestCase(SensorType.Temperature, @"deviceId\temperature\historical.zip")]
        public void GetHistoricalFilePath_ValidParams_ReturnCombinedPathToHistoricalFile(SensorType @case, string sensorTypeExpectedPath)
        {
            // Arrange
            var unitUnderTest = new BlobPathProvider();
            string deviceId = "deviceId";
            var sensorType = @case;

            // Act
            var result = unitUnderTest.GetHistoricalFilePath(deviceId, sensorType);

            // Assert
            result.Should().BeEquivalentTo(sensorTypeExpectedPath);
        }
    }
}
