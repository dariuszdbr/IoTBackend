using IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.GetDeviceDailyData
{
    [TestFixture]
    public class GetDeviceDailyDataRequestHandlerTests
    {
        private GetDeviceDailyDataRequestHandlerTestsFixture _testsFixture;

        [SetUp]
        public void SetUp()
        {
            _testsFixture = new GetDeviceDailyDataRequestHandlerTestsFixture();
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void Constructor_WhenInjectedNullService_ThenThrowArgumentNullException(bool first, bool second)
        {
            // Arrange
            var blobReader = first ? null : Substitute.For<IBlobReader>();
            var sensorDataParserProvider = second ? null : Substitute.For<ISensorDataParserProvider>();

            // Act
            Action result = () => new GetDeviceDailyDataRequestHandler(blobReader, sensorDataParserProvider);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public async Task Handle_ValidParams_ShouldReceivedCallsAndReturnResponse()
        {
            // Arrange
            _testsFixture.SubstituteForExistingBlobInBlobReader();
            _testsFixture.SubstituteForParserProvider();

            var unitUnderTest = _testsFixture.CreateunitUnderTest();
            var cancellationToken = default(CancellationToken);
            var request = new GetDeviceDailyDataRequest()
            {
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.Handle(request, cancellationToken);

            // Assert
            _testsFixture.AssertAreEquivalent(result);
            _testsFixture.ReceivedCallForGetParserWithSensorTypes();
            await _testsFixture.ReceivedCallToReadAsyncWithArgs(request);
        }

        [Test]
        public async Task Handle_WhenPathToBlobNotExist_ShouldThrowDeviceDataNotFoundException()
        {
            // Arrange
            _testsFixture.SubstituteForNotExistingBlobInBlobReader();
            _testsFixture.SubstituteForParserProvider();

            var unitUnderTest = _testsFixture.CreateunitUnderTest();
            var cancellationToken = default(CancellationToken);
            var request = new GetDeviceDailyDataRequest()
            {
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            Func<Task> result = async () => await unitUnderTest.Handle(request, cancellationToken);

            // Assert
            _testsFixture.AssertThatThrowDeviceDataNotFoundException(result);
            _testsFixture.ReceivedCallForGetParserWithSensorTypes();
            await _testsFixture.ReceivedCallToReadAsyncWithArgs(request);
        }
    }

    public class GetDeviceDailyDataRequestHandlerTestsFixture
    {
        private IBlobReader _subBlobReader;
        private ISensorDataParserProvider _subSensorDataParserProvider;
        private ISensorDataParser _rainfallParser;
        private ISensorDataParser _temperatureParser;
        private ISensorDataParser _humidityParser;

        public GetDeviceDailyDataRequestHandler CreateunitUnderTest()
        {
            return new GetDeviceDailyDataRequestHandler(
                _subBlobReader,
                _subSensorDataParserProvider);
        }

        public void SubstituteForExistingBlobInBlobReader()
        {
            _subBlobReader = Substitute.For<IBlobReader>();
            _subBlobReader.ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>())
                .Returns(GetBlobReaderResult());
        }

        public void SubstituteForNotExistingBlobInBlobReader()
        {
            _subBlobReader = Substitute.For<IBlobReader>();
            _subBlobReader.ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>())
                .Returns(BlobReaderResult.CreatePathNotExistResult());
        }

        public void SubstituteForParserProvider()
        {
            _subSensorDataParserProvider = Substitute.For<ISensorDataParserProvider>();
            _temperatureParser = Substitute.For<ISensorDataParser>();
            _temperatureParser.SensorType.Returns(SensorType.Temperature);
            _humidityParser = Substitute.For<ISensorDataParser>();
            _humidityParser.SensorType.Returns(SensorType.Humidity);
            _rainfallParser = Substitute.For<ISensorDataParser>();
            _rainfallParser.SensorType.Returns(SensorType.Rainfall);

            _subSensorDataParserProvider.GetParser(Arg.Any<SensorType>())
                .Returns(args =>
                {
                    switch ((SensorType)args[0])
                    {
                        case SensorType.Temperature:
                            return _temperatureParser;
                        case SensorType.Humidity:
                            return _humidityParser;
                        case SensorType.Rainfall:
                            return _rainfallParser;
                        default:
                            throw new NotSupportedSensorTypeException((SensorType)args[0]);
                    }
                });
        }

        private BlobReaderResult GetBlobReaderResult()
        {
            return BlobReaderResult.CreateResult(new List<SensorDailyDataPoint>()
            {
                new SensorDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 0), 12.2),
                new SensorDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 5), 11.2)
            });
        }

        private List<DeviceDailyDataPoint> GetExpectedResult()
        {
            return new List<DeviceDailyDataPoint>()
            {
                new DeviceDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 0), 12.2, 12.2, 12.2),
                new DeviceDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 5), 11.2, 11.2, 11.2)
            };
        }

        public async Task ReceivedCallToReadAsyncWithArgs(GetDeviceDailyDataRequest request)
        {
            await _subBlobReader.Received(3).ReadAsync(request.DeviceId, request.Date, Arg.Any<ISensorDataParser>());
        }

        public void ReceivedCallForGetParserWithSensorTypes()
        {
            _subSensorDataParserProvider.Received(1).GetParser(SensorType.Temperature);
            _subSensorDataParserProvider.Received(1).GetParser(SensorType.Humidity);
            _subSensorDataParserProvider.Received(1).GetParser(SensorType.Rainfall);
        }

        public void AssertAreEquivalent(GetDeviceDailyDataResponse result)
        {
            var expected = new GetDeviceDailyDataResponse() { Data = GetExpectedResult() };
            result.Should().BeEquivalentTo(expected);
        }


        public void AssertThatThrowDeviceDataNotFoundException(Func<Task> result)
        {
            result.Should().ThrowExactly<DeviceDataNotFoundException>();
        }
    }
}
