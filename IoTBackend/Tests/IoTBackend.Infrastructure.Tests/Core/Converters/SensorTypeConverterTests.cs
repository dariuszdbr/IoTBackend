using IoTBackend.Infrastructure.Core.Converters;
using NSubstitute;
using NUnit.Framework;
using System;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;

namespace IoTBackend.Infrastructure.Tests.Core.Converters
{
    [TestFixture]
    public class SensorTypeConverterTests
    {
        [TestCase("temperature", SensorType.Temperature)]
        [TestCase("humidity", SensorType.Humidity)]
        [TestCase("rainfall", SensorType.Rainfall)]
        public void Convert_WhenProvidedStringIsSupported_ThenReturnConvertedToEnumEquivalent(string @case, SensorType expectedType)
        {
            // Arrange
            var unitUnderTest = new SensorTypeConverter();
            string sensorType = @case;

            // Act
            var result = unitUnderTest.Convert(sensorType);

            // Assert
            result.Should().Be(expectedType);
        }

        [TestCase("not - supported - temperature")]
        public void Convert_WhenProvidedStringIsNotSupported_ThenThrowNotSupportedSensorTypeException(string @case)
        {
            // Arrange
            var unitUnderTest = new SensorTypeConverter();
            string sensorType = @case;

            // Act
            Action result = () => unitUnderTest.Convert(sensorType);

            // Assert
            result.Should().ThrowExactly<NotSupportedSensorTypeException>();
        }
    }
}
