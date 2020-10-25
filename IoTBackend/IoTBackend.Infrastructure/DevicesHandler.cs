using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IoTBackend.Core.Interfaces;

namespace IoTBackend.Infrastructure
{
    public interface IDevicesHandler
    {
        Task<List<ISensorDataPoint>> HandleGetDeviceSensorTypeDataDaily(GetSensorTypeDailyDataRequest request);
    }

    public class DevicesHandler : IDevicesHandler
    {
        private readonly IEnumerable<IBlobReader> _blobReaders;
        private readonly ISensorTypeConverter _converter;
        private readonly ISensorDataParserProvider _sensorDataParserProvider;

        public DevicesHandler(IEnumerable<IBlobReader> blobReaders, ISensorTypeConverter converter, ISensorDataParserProvider sensorDataParserProvider)
        {
            _blobReaders = blobReaders;
            _converter = converter;
            _sensorDataParserProvider = sensorDataParserProvider;
        }

        public async Task<List<ISensorDataPoint>> HandleGetDeviceSensorTypeDataDaily(GetSensorTypeDailyDataRequest request)
        {
            var sensorType = _converter.Convert(request.SensorType);
            var sensorDataParser = _sensorDataParserProvider.GetParser(sensorType);
            var res = new List<ISensorDataPoint>();

            foreach (var reader in _blobReaders)
            {
                var result = await reader.ReadAsync(request.DeviceId, request.Date, sensorDataParser);

                if (result == null) continue;

                res = result;
                break;
            }

            return res;
        }
    }
}