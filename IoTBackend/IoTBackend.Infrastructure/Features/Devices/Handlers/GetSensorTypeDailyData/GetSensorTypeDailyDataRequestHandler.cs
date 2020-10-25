using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Features.Devices.Dtos;
using IoTBackend.Infrastructure.Features.Devices.Interfaces;
using IoTBackend.Infrastructure.Features.Devices.Providers;
using IoTBackend.Infrastructure.Shared.Converters;
using MediatR;

namespace IoTBackend.Infrastructure.Features.Devices.Handlers.GetSensorTypeDailyData
{
    public class GetSensorTypeDailyDataRequestHandler : IRequestHandler<GetSensorTypeDailyDataRequest, GetSensorTypeDailyDataRequestResponse>
    {
        private readonly IEnumerable<IBlobReader> _blobReaders;
        private readonly ISensorTypeConverter _converter;
        private readonly ISensorDataParserProvider _sensorDataParserProvider;

        public GetSensorTypeDailyDataRequestHandler(IEnumerable<IBlobReader> blobReaders, ISensorTypeConverter converter, ISensorDataParserProvider sensorDataParserProvider)
        {
            _blobReaders = blobReaders;
            _converter = converter;
            _sensorDataParserProvider = sensorDataParserProvider;
        }

        public async Task<GetSensorTypeDailyDataRequestResponse> Handle(GetSensorTypeDailyDataRequest request, CancellationToken cancellationToken)
        {
            var sensorType = _converter.Convert(request.SensorType);
            var sensorDataParser = _sensorDataParserProvider.GetParser(sensorType);
            
            IEnumerable<ISensorDataPoint> res = new List<ISensorDataPoint>();
            foreach (var reader in _blobReaders)
            {
                var result = await reader.ReadAsync(request.DeviceId, request.Date, sensorDataParser);

                if (result == null) continue;

                res = result;
                break;
            }

            return new GetSensorTypeDailyDataRequestResponse()
            {
                Data = res.Select(sensorData => new SensorDataPointDto()
                                      {Date = sensorData.Date, Value = sensorData.Value})
            };
        }

    }
}