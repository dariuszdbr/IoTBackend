using System;
using System.Collections.Generic;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;

namespace IoTBackend.Infrastructure.Features.Devices.Shared.Readers
{
    public class BlobReaderResult
    {
        public static BlobReaderResult CreatePathNotExistResult()
        {
            return new BlobReaderResult(false, new List<SensorDailyDataPoint>());
        }

        public static BlobReaderResult CreateResult(List<SensorDailyDataPoint> result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return new BlobReaderResult(true, result);
        }

        public bool PathExist { get; private set; }
        public List<SensorDailyDataPoint> DataPoints { get; private set; }


        private BlobReaderResult(bool pathExist, List<SensorDailyDataPoint> dataPoints)
        {
            PathExist = pathExist;
            DataPoints = dataPoints;
        }

        
    }
}