using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using IoTBackend.Infrastructure.Shared.Models;
using MediatR;

namespace IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData
{
    public class GetDeviceDailyDataRequestHandler : IRequestHandler<GetDeviceDailyDataRequest, GetDeviceDailyDataResponse>
    {
        private readonly IBlobReader _blobReader;
        private readonly ISensorDataParserProvider _sensorDataParserProvider;

        public GetDeviceDailyDataRequestHandler(IBlobReader blobReader, ISensorDataParserProvider sensorDataParserProvider)
        {
            _blobReader = blobReader;
            _sensorDataParserProvider = sensorDataParserProvider;
        }

        public async Task<GetDeviceDailyDataResponse> Handle(GetDeviceDailyDataRequest request, CancellationToken cancellationToken)
        {
            var temperatureTask = _blobReader.ReadAsync(request.DeviceId, request.Date, _sensorDataParserProvider.GetParser(SensorType.Temperature));
            var humidityTask = _blobReader.ReadAsync(request.DeviceId, request.Date, _sensorDataParserProvider.GetParser(SensorType.Humidity));
            var rainfallTask = _blobReader.ReadAsync(request.DeviceId, request.Date, _sensorDataParserProvider.GetParser(SensorType.Rainfall));
            
            await Task.WhenAll(temperatureTask, humidityTask, rainfallTask);

            var temperatures = await temperatureTask;
            var humidities = await humidityTask;
            var rainfalls = await rainfallTask;

            if (temperatures == null) throw new DeviceDataNotFoundException();
            if (humidities == null) throw new DeviceDataNotFoundException();
            if (rainfalls == null) throw new DeviceDataNotFoundException();

            var deviceDataPoints = from temperature in temperatures
                join humidity in humidities on temperature.Date equals humidity.Date
                join rainfall in rainfalls on humidity.Date equals rainfall.Date
                select new DeviceDailyDataPoint(humidity.Date, temperature.Value, humidity.Value, rainfall.Value);
            
            return new GetDeviceDailyDataResponse()
            {
                Data = deviceDataPoints
            };
        }
    }
}