using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FluentAssertions;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Readers
{
    [TestFixture]
    public class BlobReaderResultTests
    {
        [Test]
        public void CreatePathNotExistResult_ThenReturnBlobReaderResultWithEmptyListAndPathExistPropertyEqualsFalse()
        {
            // Act
            var result = BlobReaderResult.CreatePathNotExistResult();

            // Assert
            result.DataPoints.Should().BeEmpty();
            result.PathExist.Should().BeFalse();
        }

        [Test]
        public void CreateResult_WhenProvidedNullArgument_ThenThrowArgumentNullException()
        {
            // Arrange
            List<SensorDailyDataPoint> list = null;

            // Act
            Action result = () => BlobReaderResult.CreateResult(list);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void CreateResult_WhenProvidedValidListArgument_ThenReturnBlobReaderResultWithNotEmptyListAndPathExistPropertyEqualsTrue()
        {
            // Arrange
            List<SensorDailyDataPoint> list = new List<SensorDailyDataPoint>()
            {
                new SensorDailyDataPoint(new DateTime(), new double())
            };
            
            // Act
            var result = BlobReaderResult.CreateResult(list);

            // Assert
            result.DataPoints.Should().BeEquivalentTo(list);
            result.PathExist.Should().BeTrue();
        }
    }
}
