using System;
using System.Collections.Generic;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Providers;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using NSubstitute;

namespace IoTBackend.Infrastructure.Tests.Features.Devices.Shared.Parsers
{
    [TestFixture]
    public class StreamParserTests
    {
        private StreamParserTestsFixture _testsFixture;

        [SetUp]
        public void SetUp()
        {
            _testsFixture = new StreamParserTestsFixture();
        }

        [Test]
        public void ParseStream_WhenProvidedNullStream_ThenThrowsArgumentNullException()
        {
            // Arrange
            var unitUnderTest = new StreamParser(_testsFixture.SubstituteForStreamReaderProvider());
            ISensorDataParser parser = _testsFixture.SubstituteForSensorDataParser();
            Stream stream = null;

            // Act
            Func<Task> result = async () => await unitUnderTest.ParseStreamAsync(parser, stream);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void ParseStream_WhenProvidedNullParser_ThenThrowsArgumentNullException()
        {
            // Arrange
            var unitUnderTest = new StreamParser(_testsFixture.SubstituteForStreamReaderProvider());
            ISensorDataParser parser = null;
            Stream stream = _testsFixture.SubstituteForStream(); ;

            // Act
            Func<Task> result = async () => await unitUnderTest.ParseStreamAsync(parser, stream);

            // Assert
            result.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public async Task ParseStream_WhenProvidedValidParams_ThenReturnParsedData()
        {
            // Arrange
            var unitUnderTest = new StreamParser(_testsFixture.SubstituteForStreamReaderProvider());
            ISensorDataParser parser = _testsFixture.SubstituteForSensorDataParser();
            Stream stream = _testsFixture.SubstituteForStream();
            var expected = new List<SensorDailyDataPoint>()
            {
                new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 5), 12.42),
                new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 10), 12.70),
                new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 15), 12.30),
                new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 20), 12.19),
                new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 25), 12.29)
            };

            // Act
            var result = await unitUnderTest.ParseStreamAsync(
                parser,
                stream);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class StreamParserTestsFixture
    {
        private IStreamReaderProvider _streamReaderProvider;
        private ISensorDataParser _sensorDataParser;

        public Stream SubstituteForStream()
        {
            return Substitute.For<Stream>();
        }

        public ISensorDataParser SubstituteForSensorDataParser()
        {
            _sensorDataParser = Substitute.For<ISensorDataParser>();
            _sensorDataParser.Parse(Arg.Any<string>())
                .Returns(
                    new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 5), 12.42),
                    new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 10), 12.70),
                    new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 15), 12.30),
                    new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 20), 12.19),
                    new SensorDailyDataPoint(new DateTime(2020, 10, 31, 10, 1, 25), 12.29)
                );

            return _sensorDataParser;
        }

        public IStreamReaderProvider SubstituteForStreamReaderProvider()
        {
            _streamReaderProvider = Substitute.For<IStreamReaderProvider>();

            var streamLines = new StringBuilder()
                .Append("2020-10-31T10:01:05;12,42")
                .Append(Environment.NewLine)
                .Append("2020-10-31T10:01:10;12,70")
                .Append(Environment.NewLine)
                .Append("2020-10-31T10:01:15;12,30")
                .Append(Environment.NewLine)
                .Append("2020-10-31T10:01:20;12,19")
                .Append(Environment.NewLine)
                .Append("2020-10-31T10:01:25;12,29")
                .Append(Environment.NewLine)
                .ToString();

            _streamReaderProvider.GetReader(Arg.Any<Stream>())
                .Returns(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(streamLines))));

            return _streamReaderProvider;
        }
    }
}
