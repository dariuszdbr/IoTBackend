using System;
using System.Collections.Generic;
using IoTBackend.Infrastructure.Core.Converters;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using NSubstitute;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.GetSensorTypeDailyData
{
    [TestFixture]
    public class GetSensorDailyDataRequestHandlerTests
    {
        private GetSensorDailyDataRequestHandlerTestsFixture _testsFixture;

        [SetUp]
        public void SetUp()
        {
            _testsFixture = new GetSensorDailyDataRequestHandlerTestsFixture();
        }

        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, true)]
        public void Constructor_WhenInjectedNullService_ThenThrowArgumentNullException(bool first, bool second, bool third)
        {
            // Arrange
            var blobReader = first ? null : Substitute.For<IBlobReader>();
            var sensorTypeConverter = second ? null : Substitute.For<ISensorTypeConverter>();
            var sensorDataParserProvider = third ? null : Substitute.For<ISensorDataParserProvider>();

            // Act
            Action result = () => new GetSensorDailyDataRequestHandler(blobReader, sensorTypeConverter, sensorDataParserProvider);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [TestCase("temperature", SensorType.Temperature)]
        [TestCase("humidity", SensorType.Humidity)]
        [TestCase("rainfall", SensorType.Rainfall)]
        public async Task Handle_ValidParams_ShouldReceivedCallsAndReturnResponse(string @case, SensorType expectedType)
        {
            // Arrange
            _testsFixture.SubstituteForExistingBlobInBlobReader();
            _testsFixture.SubstituteForISensorTypeConverter();
            _testsFixture.SubstituteForParserProvider();

            var unitUnderTest = _testsFixture.CreateGetSensorDailyDataRequestHandler();
            var cancellationToken = default(CancellationToken);
            var request = new GetSensorDailyDataRequest()
            {
                SensorType = @case,
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.Handle(request, cancellationToken);

            // Assert
            _testsFixture.AssertAreEquivalent(result);
            _testsFixture.ReceivedCallForGetParserWithSensorType(expectedType);
            _testsFixture.ReceivedCallToConvertFromString(@case);
            await _testsFixture.ReceivedCallToReadAsyncWithArgs(request);
        }

        [TestCase("not-supported-sensor-type")]
        [TestCase("")]
        [TestCase("\t")]
        public async Task Handle_WhenPassNotSupportedSensorType_ShouldThrowNotSupportedSensorTypeException(string @case)
        {
            // Arrange
            _testsFixture.SubstituteForExistingBlobInBlobReader();
            _testsFixture.SubstituteForISensorTypeConverter();
            _testsFixture.SubstituteForParserProvider();

            var unitUnderTest = _testsFixture.CreateGetSensorDailyDataRequestHandler();
            var cancellationToken = default(CancellationToken);
            var request = new GetSensorDailyDataRequest()
            {
                SensorType = @case,
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            Func<Task> result = async () => await unitUnderTest.Handle(request, cancellationToken);

            // Assert
            _testsFixture.AssertThatThrowNotSupportedSensorTypeException(result);
            _testsFixture.ReceivedCallToConvertFromString(@case);
            _testsFixture.DidNotReceivedCallForGetParserWithSensorType();
            await _testsFixture.DidNotReceivedCallToReadAsync();
        }


        [TestCase("temperature", SensorType.Temperature)]
        [TestCase("humidity", SensorType.Humidity)]
        [TestCase("rainfall", SensorType.Rainfall)]
        public async Task Handle_WhenPathToBlobNotExist_ShouldThrowDeviceDataNotFoundException(string @case, SensorType expectedType)
        {
            // Arrange
            _testsFixture.SubstituteForNotExistingBlobInBlobReader();
            _testsFixture.SubstituteForISensorTypeConverter();
            _testsFixture.SubstituteForParserProvider();

            var unitUnderTest = _testsFixture.CreateGetSensorDailyDataRequestHandler();
            var cancellationToken = default(CancellationToken);
            var request = new GetSensorDailyDataRequest()
            {
                SensorType = @case,
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            Func<Task> result = async () => await unitUnderTest.Handle(request, cancellationToken);

            // Assert
            _testsFixture.AssertThatThrowDeviceDataNotFoundException(result);
            _testsFixture.ReceivedCallToConvertFromString(@case);
            _testsFixture.ReceivedCallForGetParserWithSensorType(expectedType);
            await _testsFixture.ReceivedCallToReadAsyncWithArgs(request);
        }

    }

    public class GetSensorDailyDataRequestHandlerTestsFixture
    {
        private IBlobReader _subBlobReader;
        private ISensorTypeConverter _subSensorTypeConverter;
        private ISensorDataParserProvider _subSensorDataParserProvider;
        private ISensorDataParser _parser;

        public GetSensorDailyDataRequestHandler CreateGetSensorDailyDataRequestHandler()
        {
            return new GetSensorDailyDataRequestHandler(
                _subBlobReader,
                _subSensorTypeConverter,
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
            _parser = Substitute.For<ISensorDataParser>();
            _subSensorDataParserProvider.GetParser(Arg.Any<SensorType>())
                .Returns(args =>
                {
                    _parser.SensorType.Returns((SensorType)args[0]);
                    return _parser;
                });
        }

        public void SubstituteForISensorTypeConverter()
        {
            _subSensorTypeConverter = Substitute.For<ISensorTypeConverter>();

            _subSensorTypeConverter
                .Convert(Arg.Is<string>(arg => arg == "temperature"))
                .Returns(SensorType.Temperature);
            _subSensorTypeConverter
                .Convert(Arg.Is<string>(arg => arg == "humidity"))
                .Returns(SensorType.Humidity);
            _subSensorTypeConverter
                .Convert(Arg.Is<string>(arg => arg == "rainfall"))
                .Returns(SensorType.Rainfall);

            _subSensorTypeConverter
                .Convert(Arg.Is<string>(arg => arg != "rainfall" && arg != "humidity" && arg != "temperature"))
                .Returns(x => throw new NotSupportedSensorTypeException((string)x[0]));
        }

        private BlobReaderResult GetBlobReaderResult()
        {
            return BlobReaderResult.CreateResult(new List<SensorDailyDataPoint>()
            {
                new SensorDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 0), 12.2),
                new SensorDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 5), 12.2),
                new SensorDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 10), 12.2),
                new SensorDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 15), 12.2),
                new SensorDailyDataPoint(new DateTime(2020, 11, 1, 13, 30, 20), 12.2)
            });
        }

        public async Task ReceivedCallToReadAsyncWithArgs(GetSensorDailyDataRequest request)
        {
            await _subBlobReader.Received(1).ReadAsync(request.DeviceId, request.Date, _parser);
        }

        public void ReceivedCallToConvertFromString(string @case)
        {
            _subSensorTypeConverter.Received(1).Convert(@case);
        }

        public void ReceivedCallForGetParserWithSensorType(SensorType expectedType)
        {
            _subSensorDataParserProvider.Received(1).GetParser(expectedType);
        }

        public void AssertAreEquivalent(GetSensorDailyDataResponse result)
        {
            var expected = new GetSensorDailyDataResponse { Data = GetBlobReaderResult().DataPoints };
            result.Should().BeEquivalentTo(expected);
        }

        public void AssertThatThrowNotSupportedSensorTypeException(Func<Task> result)
        {
            result.Should().ThrowExactly<NotSupportedSensorTypeException>();
        }

        public void DidNotReceivedCallForGetParserWithSensorType()
        {
            _subSensorDataParserProvider.DidNotReceive().GetParser(Arg.Any<SensorType>());
        }

        public async Task DidNotReceivedCallToReadAsync()
        {
            await _subBlobReader.DidNotReceive().ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>());
        }

        public void AssertThatThrowDeviceDataNotFoundException(Func<Task> result)
        {
            result.Should().ThrowExactly<DeviceDataNotFoundException>();
        }
    }
}
