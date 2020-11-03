using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Providers
{
    [TestFixture]
    public class SensorDataParserProviderTests
    {
        private IEnumerable<ISensorDataParser> _subEnumerable;

        [SetUp]
        public void SetUp()
        {
            var parser1 = Substitute.For<ISensorDataParser>();
            var parser2 = Substitute.For<ISensorDataParser>();
            var parser3 = Substitute.For<ISensorDataParser>();
            parser1.SensorType.Returns(SensorType.Temperature);
            parser2.SensorType.Returns(SensorType.Humidity);
            parser3.SensorType.Returns(SensorType.Rainfall);
            
            _subEnumerable = new List<ISensorDataParser>()
            {
                parser1, parser2, parser3
            };
        }

        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Rainfall)]
        public void GetParser_WhenProvidedSupportedSensorType_ThenReturnCorrespondingParserType(SensorType @case)
        {
            // Arrange
            var provider = new SensorDataParserProvider(_subEnumerable);
            SensorType sensorType = @case;

            // Act
            var result = provider.GetParser(sensorType);

            // Assert
            result.SensorType.Should().Be(@case);
        }

        [Test]
        public void GetParser_WhenProvidedNotSupportedSensorType_ThenThrowNotSupportedSensorTypeException()
        {
            // Arrange
            var provider = new SensorDataParserProvider(_subEnumerable);
            SensorType notDefinedSensorType = (SensorType)999999;

            // Act
            Action result = () => provider.GetParser(notDefinedSensorType);

            // Assert
            result.Should().ThrowExactly<NotSupportedSensorTypeException>();
        }


    }
}
