using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using NUnit.Framework;
using System;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Parsers
{
    [TestFixture]
    public class RainfallSensorDailyDataPointParserTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public void Parse_WhenProvidedNullEmptyOrWhiteSpace_ThenThrowArgumentException(string @case)
        {
            // Arrange
            var unitUnderTest = new RainfallSensorDailyDataPointParser();

            // Act
            Action result = () => unitUnderTest.Parse(@case);

            // Assert
            result.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Parse_WhenProvidedValidLineOfData_ThenReturnNewSensorDailyDataPoint()
        {
            // Arrange
            var unitUnderTest = new RainfallSensorDailyDataPointParser();
            var line = "2020-10-31T00:00:00;9.12";
            var expected = new SensorDailyDataPoint(new DateTime(2020, 10, 31, 0, 0, 0), 9.12);

            // Act
            var result = unitUnderTest.Parse(line);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void SensorType_MustBeRainfall()
        {
            // Arrange
            var unitUnderTest = new RainfallSensorDailyDataPointParser();
            var expected = SensorType.Rainfall;

            // Assert
            unitUnderTest.SensorType.Should().Be(expected);
        }
    }
}
