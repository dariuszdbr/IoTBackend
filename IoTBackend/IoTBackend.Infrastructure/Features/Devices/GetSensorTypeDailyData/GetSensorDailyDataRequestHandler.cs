using System;
using System.Threading;
using System.Threading.Tasks;
using IoTBackend.Infrastructure.Core.Converters;
using IoTBackend.Infrastructure.Features.Devices.Shared.Exceptions;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using MediatR;

namespace IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData
{
    public class GetSensorDailyDataRequestHandler : IRequestHandler<GetSensorDailyDataRequest, GetSensorDailyDataResponse>
    {
        private readonly IBlobReader _blobReader;
        private readonly ISensorTypeConverter _converter;
        private readonly ISensorDataParserProvider _sensorDataParserProvider;

        public GetSensorDailyDataRequestHandler(IBlobReader blobReader, ISensorTypeConverter converter, ISensorDataParserProvider sensorDataParserProvider)
        {
            _blobReader = blobReader ?? throw new ArgumentNullException(nameof(blobReader));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _sensorDataParserProvider = sensorDataParserProvider ?? throw new ArgumentNullException(nameof(sensorDataParserProvider));
        }

        public async Task<GetSensorDailyDataResponse> Handle(GetSensorDailyDataRequest request, CancellationToken cancellationToken)
        {
            var sensorType = _converter.Convert(request.SensorType);
            var sensorDataParser = _sensorDataParserProvider.GetParser(sensorType);
            var res = await _blobReader.ReadAsync(request.DeviceId, request.Date, sensorDataParser);

            if (!res.PathExist) throw new DeviceDataNotFoundException();

            return new GetSensorDailyDataResponse()
            {
                Data = res.DataPoints
            };
        }
    }
}