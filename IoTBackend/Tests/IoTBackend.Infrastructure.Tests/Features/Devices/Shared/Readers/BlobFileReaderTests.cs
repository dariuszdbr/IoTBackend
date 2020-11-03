using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Core.Providers;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Readers
{
    [TestFixture]
    public class BlobFileReaderTests
    {
        private BlobFileReaderTestsFixture _testsFixture;
        [SetUp]
        public void SetUp()
        {
            _testsFixture = new BlobFileReaderTestsFixture();
        }

        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(true, false, true)]
        public void Constructor_WhenInjectedNullService_ThenThrowArgumentNullException(bool first, bool second, bool third)
        {
            // Arrange
            var blobClientProvider = first ? null : Substitute.For<IBlobClientProvider>();
            var blobPathProvider = second ? null : Substitute.For<IBlobPathProvider>();
            var streamParser = third ? null : Substitute.For<IStreamParser>();

            // Act
            Action result = () => new BlobFileReader(blobClientProvider, blobPathProvider, streamParser);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Rainfall)]
        public async Task ReadAsync_WhenProvidedNotExistingDeviceDateOrSensorType_ThenShouldReturnEmptyBlobReaderResult(SensorType @case)
        {
            // Arrange
            var unitUnderTest = _testsFixture.ArrangeForNotExistingDeviceDateOrSensorType(_testsFixture.GetExistingDeviceId(), _testsFixture.GetDate(), @case);

            var deviceId = _testsFixture.GetNotExistingDeviceId();
            var dateTime = _testsFixture.GetDate();
             var parser = _testsFixture.GetSensorDataParser();
            
            // Act
            var result = await unitUnderTest.ReadAsync(deviceId, dateTime, parser);

            // Assert
            _testsFixture.AssertNotExistingDeviceOrNotValidDate(result);
        }

        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Rainfall)]
        public async Task ReadAsync_WhenProvidedExistingDeviceAndValidDate_ThenShouldReturnBlobReaderResult(SensorType @case)
        {
            // Arrange
            var unitUnderTest = _testsFixture.ArrangeForExistingDeviceDateAndSensorType(_testsFixture.GetExistingDeviceId(), _testsFixture.GetDate(), @case);

            var deviceId = _testsFixture.GetExistingDeviceId();
            var dateTime = _testsFixture.GetDate();
            var parser = _testsFixture.GetSensorDataParser();

            // Act
            var result = await unitUnderTest.ReadAsync(deviceId, dateTime, parser);

            // Assert
            _testsFixture.AssertExistingDeviceAndValidDate(result);
        }
    }

    public class BlobFileReaderTestsFixture
    {
        private ISensorDataParser _subSensorDataParser;
        private IBlobPathProvider _subBlobPathProvider;
        private Stream _subStream;
        private BlobClient _blobClient;
        private IBlobClientProvider _subBlobClientProvider;
        private IStreamParser _subStreamParser;
        private readonly DateTime _dateTime = new DateTime(2021, 11, 2, 1,2,3);
        private string _notExistingDeviceId = "not-existing-device";
        private string _existingDeviceId = "existing-device";

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

        public ISensorDataParser GetSensorDataParser()
        {
            return _subSensorDataParser;
        }


        public BlobFileReader ArrangeForExistingDeviceDateAndSensorType(string deviceId, DateTime dateTime, SensorType sensorType)
        {
            _subBlobPathProvider = SubstituteForBlobPathProvider(deviceId, dateTime, sensorType);
            _subStream = Substitute.For<Stream>();
            _subBlobClientProvider = SubstituteForExistingBlobInBlobClientProvider(dateTime);
            _subSensorDataParser = Substitute.For<ISensorDataParser>();
            _subSensorDataParser.SensorType.Returns(sensorType);
            _subStreamParser = Substitute.For<IStreamParser>();
            _subStreamParser.ParseStreamAsync(_subSensorDataParser, _subStream).Returns(GetBlobReaderResult().DataPoints);

            return new BlobFileReader(_subBlobClientProvider, _subBlobPathProvider, _subStreamParser);
        }

        public BlobFileReader ArrangeForNotExistingDeviceDateOrSensorType(string deviceId, DateTime dateTime, SensorType sensorType)
        {
            _subBlobPathProvider = SubstituteForBlobPathProvider(deviceId, dateTime, sensorType);
            _subBlobClientProvider = SubstituteForNotExistingBlobInBlobClientProvider();
            _subSensorDataParser = Substitute.For<ISensorDataParser>();
            _subSensorDataParser.SensorType.Returns(sensorType);
            _subStreamParser = Substitute.For<IStreamParser>();

            return new BlobFileReader(_subBlobClientProvider, _subBlobPathProvider, _subStreamParser);
        }

        private IBlobClientProvider SubstituteForExistingBlobInBlobClientProvider(DateTime dateTime)
        {
            _blobClient = Substitute.For<BlobClient>();
            _blobClient.ExistsAsync().Returns(Task.FromResult(Response.FromValue(true, default!)));
            _blobClient.OpenReadAsync().Returns(_subStream);

            _subBlobClientProvider = Substitute.For<IBlobClientProvider>();
            _subBlobClientProvider
                .GetClient(Arg.Is<string>(arg => arg == $"{_existingDeviceId}/Humidity/{dateTime:yyyy-MM-dd}" ||
                                                 arg == $"{_existingDeviceId}/Rainfall/{dateTime:yyyy-MM-dd}" ||
                                                 arg == $"{_existingDeviceId}/Temperature/{dateTime:yyyy-MM-dd}"))
                .Returns(_blobClient);
            return _subBlobClientProvider;
        }

        private IBlobPathProvider SubstituteForBlobPathProvider(string deviceId, DateTime dateTime,
            SensorType sensorType)
        {
            _subBlobPathProvider = Substitute.For<IBlobPathProvider>();
            _subBlobPathProvider.GetFilePath(deviceId, dateTime, sensorType)
                .Returns(args => $"{deviceId}/{sensorType}/{dateTime:yyyy-MM-dd}");
            return _subBlobPathProvider;
        }

        private IBlobClientProvider SubstituteForNotExistingBlobInBlobClientProvider()
        {
            _blobClient = Substitute.For<BlobClient>();
            _blobClient.ExistsAsync().Returns(Task.FromResult(Response.FromValue(false, default!)));

            _subBlobClientProvider = Substitute.For<IBlobClientProvider>();
            _subBlobClientProvider.GetClient(Arg.Any<string>()).Returns(_blobClient);

            return _subBlobClientProvider;
        }

        public void AssertNotExistingDeviceOrNotValidDate(BlobReaderResult result)
        {
            _subBlobPathProvider.Received(1).GetFilePath(_notExistingDeviceId, _dateTime, Arg.Any<SensorType>());
            _subBlobClientProvider.Received(1).GetClient(Arg.Any<string>());
            _blobClient.DidNotReceive().OpenReadAsync();
            _subStreamParser.DidNotReceive().ParseStreamAsync(Arg.Any<ISensorDataParser>(), Arg.Any<Stream>());
            result.DataPoints.Should().BeEmpty();
            result.PathExist.Should().BeFalse();
        }

        public string GetNotExistingDeviceId()
        {
            return _notExistingDeviceId;
        }

        public void AssertExistingDeviceAndValidDate(BlobReaderResult result)
        {
            _subBlobPathProvider.Received(1).GetFilePath(_existingDeviceId, _dateTime, Arg.Any<SensorType>());
            _subBlobClientProvider.Received(1).GetClient(Arg.Any<string>());
            _blobClient.Received(1).OpenReadAsync();
            _subStreamParser.Received(1).ParseStreamAsync(Arg.Any<ISensorDataParser>(), Arg.Any<Stream>());
            result.Should().BeEquivalentTo(GetBlobReaderResult());
        }

        public string GetExistingDeviceId()
        {
            return _existingDeviceId;
        }

        public DateTime GetDate()
        {
            return _dateTime;
        }
    }
}
