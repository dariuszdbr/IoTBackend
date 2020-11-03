using IoTBackend.Infrastructure.Core.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Readers
{
    [TestFixture]
    public class BlobArchiveReaderTests
    {
        private BlobArchiveReaderTestsFixture _testsFixture;
        
        [SetUp]
        public void SetUp()
        {
            _testsFixture = new BlobArchiveReaderTestsFixture();
        }

        [TestCase(true, false, false, false)]
        [TestCase(false, true, false, false)]
        [TestCase(true, false, true, false)]
        [TestCase(false, false, false, true)]
        public void ReadAsync_WhenInjectedNullService_ThenThrowArgumentNullException(bool first, bool second, bool third, bool fourth)
        {
            // Arrange
            var blobClientProvider = first ? null : Substitute.For<IBlobClientProvider>();
            var blobPathProvider = second ? null : Substitute.For<IBlobPathProvider>();
            var zipArchiveProvider = third ? null : Substitute.For<IZipArchiveProvider>();
            var streamParser = fourth? null : Substitute.For<IStreamParser>();

            // Act
            Action result = () => new BlobArchiveReader(blobClientProvider, blobPathProvider, zipArchiveProvider, streamParser);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Rainfall)]
        public void ReadAsync_WhenInjectedNullParser_ThenThrowArgumentNullException(SensorType @case)
        {
            // Arrange
            var unitUnderTest = _testsFixture.ArrangeForExistingDevice(_testsFixture.GetExistingDeviceId(), @case);
            // Act
            Func<Task> result = async () => await unitUnderTest.ReadAsync("deviceId", new DateTime(), null);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Rainfall)]
        public async Task ReadAsync_WhenProvidedNotExistingDevice_ThenShouldReturnEmptyBlobReaderResult(SensorType @case)
        {
            // Arrange
            var deviceId = _testsFixture.GetNotExistingDeviceId();
            var unitUnderTest = _testsFixture.ArrangeForNotExistingDevice(deviceId, @case);
            var dateTime = _testsFixture.GetDate();
            var parser = _testsFixture.GetSensorDataParser();

            // Act
            var result = await unitUnderTest.ReadAsync(deviceId, dateTime, parser);

            // Assert
            _testsFixture.AssertNotExistingDevice(result);
        }

        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Rainfall)]
        public async Task ReadAsync_WhenProvidedExistingDeviceButNotExistingDate_ThenShouldReturnEmptyBlobReaderResult(SensorType @case)
        {
            // Arrange
            var deviceId = _testsFixture.GetExistingDeviceId();
            var unitUnderTest = _testsFixture.ArrangeForExistingDeviceButNotExistingDate(deviceId, @case);
            var dateTime = _testsFixture.GetDate();
            var parser = _testsFixture.GetSensorDataParser();

            // Act
            var result = await unitUnderTest.ReadAsync(deviceId, dateTime, parser);

            // Assert
            _testsFixture.AssertNotExistingDate(result);
        }

        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Rainfall)]
        public async Task ReadAsync_WhenProvidedExistingDeviceAndValidDate_ThenShouldReturnBlobReaderResult(SensorType @case)
        {
            // Arrange
            var deviceId = _testsFixture.GetExistingDeviceId();
            var unitUnderTest = _testsFixture.ArrangeForExistingDevice(deviceId, @case);
            var dateTime = _testsFixture.GetDate();
            var parser = _testsFixture.GetSensorDataParser();

            // Act
            var result = await unitUnderTest.ReadAsync(deviceId, dateTime, parser);

            // Assert
            _testsFixture.AssertExistingDeviceAndValidDate(result);
        }
    }

    public class BlobArchiveReaderTestsFixture
    {
        private readonly DateTime _dateTime = new DateTime(2021, 11, 2, 1, 2, 3);
        private string _notExistingDeviceId = "not-existing-device";
        private string _existingDeviceId = "existing-device";
        private ISensorDataParser _subSensorDataParser;
        private static BlobClient _blobClient;
        private static IBlobClientProvider _subBlobClientProvider;
        private IBlobPathProvider _subBlobPathProvider;
        private IStreamParser _subStreamParser;
        private IZipArchiveProvider _zipArchiveProvider;
        private DateTime _notValidDateTime;

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

        public BlobArchiveReader ArrangeForExistingDevice(string deviceId, SensorType sensorType)
        {
            var subBlobStream = Substitute.For<Stream>();
            _subBlobPathProvider = SubstituteForBlobPathProvider(deviceId, sensorType);
            _subBlobClientProvider = SubstituteForExistingBlobInBlobClientProvider(subBlobStream);
            _zipArchiveProvider = SubstituteForZipArchiveProvider(subBlobStream, $"{_dateTime:yyyy-MM-dd}.csv");
            _subStreamParser = SubstituteForValidStreamParser(sensorType);

            return new BlobArchiveReader(_subBlobClientProvider, _subBlobPathProvider, _zipArchiveProvider,  _subStreamParser);
        }

        public BlobArchiveReader ArrangeForExistingDeviceButNotExistingDate(string deviceId, SensorType sensorType)
        {
            var subBlobStream = Substitute.For<Stream>();
            _subBlobPathProvider = SubstituteForBlobPathProvider(deviceId, sensorType);
            _subBlobClientProvider = SubstituteForExistingBlobInBlobClientProvider(subBlobStream);
            _notValidDateTime = DateTime.Now;
            _zipArchiveProvider = SubstituteForZipArchiveProvider(subBlobStream, $"{_notValidDateTime:yyyy-MM-dd}.csv");
            _subStreamParser = SubstituteForValidStreamParser(sensorType);

            return new BlobArchiveReader(_subBlobClientProvider, _subBlobPathProvider, _zipArchiveProvider, _subStreamParser);
        }

        public BlobArchiveReader ArrangeForNotExistingDevice(string deviceId, SensorType sensorType)
        {
            _subBlobPathProvider = SubstituteForBlobPathProvider(deviceId, sensorType);
            _subBlobClientProvider = SubstituteForNotExistingBlobInBlobClientProvider();
            _zipArchiveProvider = Substitute.For<IZipArchiveProvider>();
            _subStreamParser = Substitute.For<IStreamParser>();
            _subSensorDataParser = Substitute.For<ISensorDataParser>();
            _subSensorDataParser.SensorType.Returns(sensorType);

            return new BlobArchiveReader(_subBlobClientProvider, _subBlobPathProvider, _zipArchiveProvider, _subStreamParser);
        }


        private IBlobPathProvider SubstituteForBlobPathProvider(string deviceId, SensorType sensorType)
        {
            _subBlobPathProvider = Substitute.For<IBlobPathProvider>();
            _subBlobPathProvider.GetHistoricalFilePath(Arg.Any<string>(), sensorType)
                .Returns(args => $"{deviceId}/{sensorType}/historical.zip");

             return _subBlobPathProvider;
        }

        private static IBlobClientProvider SubstituteForExistingBlobInBlobClientProvider(Stream subStream)
        {
            _blobClient = Substitute.For<BlobClient>();
            _blobClient.ExistsAsync().Returns(Task.FromResult(Response.FromValue(true, default!)));
            _blobClient.OpenReadAsync().Returns(subStream);

            _subBlobClientProvider = Substitute.For<IBlobClientProvider>();
            _subBlobClientProvider.GetClient(Arg.Is<string>(arg => arg == "existing-device/Humidity/historical.zip" || arg == "existing-device/Rainfall/historical.zip" || arg == "existing-device/Temperature/historical.zip"))
                .Returns(_blobClient);
            return _subBlobClientProvider;
        }

        private static IBlobClientProvider SubstituteForNotExistingBlobInBlobClientProvider()
        {
            _blobClient = Substitute.For<BlobClient>();
            _blobClient.ExistsAsync().Returns(Task.FromResult(Response.FromValue(false, default!)));

            var subBlobClientProvider = Substitute.For<IBlobClientProvider>();
            subBlobClientProvider
                .GetClient(Arg.Is<string>(arg => arg == "not-existing-device/Humidity/historical.zip" || arg == "not-existing-device/Rainfall/historical.zip" || arg == "not-existing-device/Temperature/historical.zip"))
                .Returns(_blobClient);
            return subBlobClientProvider;
        }

        private IZipArchiveProvider SubstituteForZipArchiveProvider(Stream entryStream, string entryName)
        {
            _zipArchiveProvider = Substitute.For<IZipArchiveProvider>();
            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var archiveEntry = archive.CreateEntry(entryName);
                using (var stream = archiveEntry.Open())
                {
                    entryStream.CopyTo(stream);
                }
            }

            memoryStream.Position = 0;
            _zipArchiveProvider.GetZipArchive(entryStream).Returns(new ZipArchive(memoryStream, ZipArchiveMode.Update));

            return _zipArchiveProvider;
        }

        private IStreamParser SubstituteForValidStreamParser(SensorType sensorType)
        {
            _subSensorDataParser = Substitute.For<ISensorDataParser>();
            _subSensorDataParser.SensorType.Returns(sensorType);

            _subStreamParser = Substitute.For<IStreamParser>();
            _subStreamParser.ParseStreamAsync(_subSensorDataParser, Arg.Any<Stream>()).Returns(GetBlobReaderResult().DataPoints);
            return _subStreamParser;
        }

        public string GetExistingDeviceId()
        {
            return _existingDeviceId;
        }

        public string GetNotExistingDeviceId()
        {
            return _notExistingDeviceId;
        }

        public DateTime GetDate()
        {
            return _dateTime;
        }


        // Asserts
        public void AssertExistingDeviceAndValidDate(BlobReaderResult result)
        {
            _subBlobPathProvider.Received(1).GetHistoricalFilePath(_existingDeviceId, Arg.Any<SensorType>());
            _subBlobClientProvider.Received(1).GetClient(Arg.Any<string>());
            _blobClient.Received(1).OpenReadAsync();
            _zipArchiveProvider.Received(1).GetZipArchive(Arg.Any<Stream>());
            _subStreamParser.Received(1).ParseStreamAsync(Arg.Any<ISensorDataParser>(), Arg.Any<Stream>());
            result.Should().BeEquivalentTo(GetBlobReaderResult());
        }

        public void AssertNotExistingDevice(BlobReaderResult result)
        {
            _subBlobPathProvider.Received(1).GetHistoricalFilePath(_notExistingDeviceId, Arg.Any<SensorType>());
            _subBlobClientProvider.Received(1).GetClient(Arg.Any<string>());
            _blobClient.DidNotReceive().OpenReadAsync();
            _zipArchiveProvider.DidNotReceive().GetZipArchive(Arg.Any<Stream>());
            _subStreamParser.DidNotReceive().ParseStreamAsync(Arg.Any<ISensorDataParser>(), Arg.Any<Stream>());
            result.DataPoints.Should().BeEmpty();
            result.PathExist.Should().BeFalse();
        }

        public void AssertNotExistingDate(BlobReaderResult result)
        {
            _subBlobPathProvider.Received(1).GetHistoricalFilePath(_existingDeviceId, Arg.Any<SensorType>());
            _subBlobClientProvider.Received(1).GetClient(Arg.Any<string>());
            _blobClient.Received(1).OpenReadAsync();
            _zipArchiveProvider.Received(1).GetZipArchive(Arg.Any<Stream>());
            _subStreamParser.DidNotReceive().ParseStreamAsync(Arg.Any<ISensorDataParser>(), Arg.Any<Stream>());
            result.DataPoints.Should().BeEmpty();
            result.PathExist.Should().BeFalse();
        }
        //


    }
}
