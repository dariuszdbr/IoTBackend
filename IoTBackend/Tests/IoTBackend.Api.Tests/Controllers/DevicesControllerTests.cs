using IoTBackend.Api.Controllers;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using IoTBackend.Infrastructure.Core.Exceptions;
using IoTBackend.Infrastructure.Core.Models;
using IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;

namespace IoTBackend.Api.Tests.Controllers
{
    [TestFixture]
    public class DevicesControllerTests
    {
        private IMediator subMediator;

        [SetUp]
        public void SetUp()
        {
            this.subMediator = Substitute.For<IMediator>();
        }


        [Test]
        public async Task GetSensorTypeDailyData_WhenProvidedNullParameter_ThenReturnBadRequest()
        {
            // Arrange
            var unitUnderTest = new DevicesController(this.subMediator);
            GetSensorDailyDataRequest request = null;

            // Act
            var result = await unitUnderTest.GetSensorTypeDailyData(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public async Task GetSensorTypeDailyData_WhenProvidedNullOrWhiteSpaceSensorType_ThenReturnBadRequest(string @case)
        {
            // Arrange
            var unitUnderTest = new DevicesController(this.subMediator);
            GetSensorDailyDataRequest request = new GetSensorDailyDataRequest()
            {
                SensorType = @case
            };

            // Act
            var result = await unitUnderTest.GetSensorTypeDailyData(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public async Task GetSensorTypeDailyData_WhenProvidedNullOrWhiteSpaceDeviceId_ThenReturnBadRequestResult(string @case)
        {
            // Arrange
            var unitUnderTest = new DevicesController(this.subMediator);
            GetSensorDailyDataRequest request = new GetSensorDailyDataRequest()
            {
                SensorType = "not null or empty",
                DeviceId = @case
            };

            // Act
            var result = await unitUnderTest.GetSensorTypeDailyData(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }


        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Rainfall)]
        public async Task GetSensorTypeDailyData_WhenProvidedValidRequestParameters_ThenReturnOkObjectResultWithResponseObject(SensorType @case)
        {
            // Arrange
            subMediator.Send(Arg.Any<GetSensorDailyDataRequest>()).Returns(new GetSensorDailyDataResponse());
            var unitUnderTest = new DevicesController(this.subMediator);
            var request = new GetSensorDailyDataRequest()
            {
                Date = DateTime.Now,
                SensorType = @case.ToString(),
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.GetSensorTypeDailyData(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new GetSensorDailyDataResponse());
        }

        [Test]
        public async Task GetSensorTypeDailyData_WhenMediatrThrowsDomainException_ThenReturnBadRequestResult()
        {
            // Arrange
            var expectedStatusCode = 404;
            subMediator.Send(Arg.Any<GetSensorDailyDataRequest>()).Throws(x => throw new DomainException("message", expectedStatusCode));
            var unitUnderTest = new DevicesController(this.subMediator);
            var request = new GetSensorDailyDataRequest()
            {
                Date = DateTime.Now,
                SensorType = "some not supported sensor type",
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.GetSensorTypeDailyData(request);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task GetSensorTypeDailyData_WhenMediatrThrowsSomeUnhandledException_ThenReturnBadRequestResult()
        {
            // Arrange
            var expectedStatusCode = 500;
            subMediator.Send(Arg.Any<GetSensorDailyDataRequest>()).Throws(x => throw new Exception("message"));
            var unitUnderTest = new DevicesController(this.subMediator);
            var request = new GetSensorDailyDataRequest()
            {
                Date = DateTime.Now,
                SensorType = "some not supported sensor type",
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.GetSensorTypeDailyData(request);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(expectedStatusCode);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        [Test]
        public async Task GetDeviceDailyData_WhenProvidedNullParameter_ThenReturnBadRequest()
        {
            // Arrange
            var unitUnderTest = new DevicesController(this.subMediator);
            GetDeviceDailyDataRequest request = null;

            // Act
            var result = await unitUnderTest.GetDeviceDailyData(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\t")]
        public async Task GetDeviceDailyData_WhenProvidedNullOrWhiteSpaceDeviceId_ThenReturnBadRequestResult(string @case)
        {
            // Arrange
            var unitUnderTest = new DevicesController(this.subMediator);
            GetDeviceDailyDataRequest request = new GetDeviceDailyDataRequest()
            {
                DeviceId = @case
            };

            // Act
            var result = await unitUnderTest.GetDeviceDailyData(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [TestCase(SensorType.Temperature)]
        [TestCase(SensorType.Humidity)]
        [TestCase(SensorType.Rainfall)]
        public async Task GetDeviceDailyData_WhenProvidedValidRequestParameters_ThenReturnOkObjectResultWithResponseObject(SensorType @case)
        {
            // Arrange
            subMediator.Send(Arg.Any<GetDeviceDailyDataRequest>()).Returns(new GetDeviceDailyDataResponse());
            var unitUnderTest = new DevicesController(this.subMediator);
            var request = new GetDeviceDailyDataRequest()
            {
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.GetDeviceDailyData(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new GetDeviceDailyDataResponse());
        }

        [Test]
        public async Task GetDeviceDailyData_WhenMediatrThrowsDomainException_ThenReturnBadRequestResult()
        {
            // Arrange
            var expectedStatusCode = 404;
            subMediator.Send(Arg.Any<GetDeviceDailyDataRequest>()).Throws(x => throw new DomainException("message", expectedStatusCode));
            var unitUnderTest = new DevicesController(this.subMediator);
            var request = new GetDeviceDailyDataRequest()
            {
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.GetDeviceDailyData(request);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(expectedStatusCode);
        }

        [Test]
        public async Task GetDeviceDailyData_WhenMediatrThrowsSomeUnhandledException_ThenReturnBadRequestResult()
        {
            // Arrange
            var expectedStatusCode = 500;
            subMediator.Send(Arg.Any<GetDeviceDailyDataRequest>()).Throws(x => throw new Exception("message"));
            var unitUnderTest = new DevicesController(this.subMediator);
            var request = new GetDeviceDailyDataRequest()
            {
                Date = DateTime.Now,
                DeviceId = "deviceId"
            };

            // Act
            var result = await unitUnderTest.GetDeviceDailyData(request);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
