using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Readers
{
    [TestFixture]
    public class BlobReaderTests
    {
        private BlobReaderTestsFixture _testsFixture;

        [SetUp]
        public void SetUp()
        {
            _testsFixture = new BlobReaderTestsFixture();
        }

        [Test]
        public async Task ReadAsync_WhenFirstReaderReturnEmptyResult_ThenWillSearchInSecondReader()
        {
            // Arrange
            _testsFixture.SubstituteFileReaderReadFromNotExistingPath();
            _testsFixture.SubstituteArchiveReaderReadFromExistingPath();

            var blobReader = _testsFixture.CreateBlobReader();

            string deviceId = "existing-device";
            DateTime dateTime = new DateTime();
            ISensorDataParser parser = Substitute.For<ISensorDataParser>();

            // Act
            var result = await blobReader.ReadAsync(
                deviceId,
                dateTime,
                parser);

            // Assert
            await _testsFixture.AssertReceivedCallForFileReader();
            await _testsFixture.AssertReceivedCallForArchiveReader();
            _testsFixture.AssertThatResultIsEquivalent(result);
        }

        [Test]
        public async Task ReadAsync_WhenFirstReaderReturnResult_ThenWillNotSearchInSecondReader()
        {
            // Arrange
            _testsFixture.SubstituteFileReaderReadFromExistingPath();
            _testsFixture.SubstituteArchiveReaderReadFromNotExistingPath();

            var blobReader = _testsFixture.CreateBlobReader();

            string deviceId = "existing-device";
            DateTime dateTime = new DateTime();
            ISensorDataParser parser = Substitute.For<ISensorDataParser>();

            // Act
            var result = await blobReader.ReadAsync(
                deviceId,
                dateTime,
                parser);

            // Assert
             await _testsFixture.AssertReceivedCallForFileReader();
             await _testsFixture.AssertDidNotReceivedCallForArchiveReader();
             _testsFixture.AssertThatResultIsEquivalent(result);
        }

        [Test]
        public async Task ReadAsync_WhenInjectedEmptyListOfBlobReaders_ThenReturnEmptyResult()
        {
            // Arrange
            var blobReader = _testsFixture.CreateEmptyBlobReader();

            string deviceId = "existing-device";
            DateTime dateTime = new DateTime();
            ISensorDataParser parser = Substitute.For<ISensorDataParser>();

            // Act
            var result = await blobReader.ReadAsync(
                deviceId,
                dateTime,
                parser);

            // Assert
            await _testsFixture.AssertDidNotReceivedCallForFileReader();
            await _testsFixture.AssertDidNotReceivedCallForArchiveReader();
            result.DataPoints.Should().BeEmpty();
            result.PathExist.Should().BeFalse();
        }

        [Test]
        public void ReadAsync_WhenInjectedNullList_ThenThrowArgumentNullException()
        {
            // Arrange
            // Act
            Action result = () => new BlobReader(null);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }


    }

    public class BlobReaderTestsFixture
    {
        private IEnumerable<IBlobReader> subEnumerable;
        private IBlobReader _fileBlobReader;
        private IBlobReader _archiveBlobReader;

        public BlobReader CreateBlobReader()
        {
            this.subEnumerable = new[] { _fileBlobReader, _archiveBlobReader };

            return new BlobReader(this.subEnumerable);
        }
        public BlobReader CreateEmptyBlobReader()
        {
            _archiveBlobReader = Substitute.For<IBlobReader>();
            _fileBlobReader = Substitute.For<IBlobReader>();

            this.subEnumerable = new IBlobReader[] { };

            return new BlobReader(this.subEnumerable);
        }


        public void SubstituteArchiveReaderReadFromExistingPath()
        {
            _archiveBlobReader = Substitute.For<IBlobReader>();
            _archiveBlobReader.ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>())
                .Returns(Task.FromResult(GetBlobReaderResult()));
        }
        public void SubstituteArchiveReaderReadFromNotExistingPath()
        {
            _archiveBlobReader = Substitute.For<IBlobReader>();
            _archiveBlobReader.ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>())
                .Returns(Task.FromResult(BlobReaderResult.CreatePathNotExistResult()));
        }

        public void SubstituteFileReaderReadFromExistingPath()
        {
            _fileBlobReader = Substitute.For<IBlobReader>();
            _fileBlobReader.ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>())
                .Returns(Task.FromResult(GetBlobReaderResult()));
        }
        public void SubstituteFileReaderReadFromNotExistingPath()
        {
            _fileBlobReader = Substitute.For<IBlobReader>();
            _fileBlobReader.ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>())
                .Returns(Task.FromResult(BlobReaderResult.CreatePathNotExistResult()));
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
        

        public void AssertThatResultIsEquivalent(BlobReaderResult result)
        {
            result.Should().BeEquivalentTo(GetBlobReaderResult());
        }

        public async Task AssertReceivedCallForFileReader()
        {
            await _fileBlobReader.Received(1).ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>());
        }

        public async Task AssertDidNotReceivedCallForFileReader()
        {
            await _fileBlobReader.DidNotReceive().ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>());
        }

        public async Task AssertReceivedCallForArchiveReader()
        {
            await _archiveBlobReader.Received(1)
                .ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>());
        }

        public async Task AssertDidNotReceivedCallForArchiveReader()
        {
            await _archiveBlobReader.DidNotReceive()
                .ReadAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<ISensorDataParser>());
        }
    }
}
