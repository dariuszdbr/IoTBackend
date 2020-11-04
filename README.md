# IoTBackend .NET

This repo contains a sample application written in C# that exposes API to communicate with Azure Blob Storage where IoT data from the weather station is stored.

Storage contains CSV files that are stored daily within `{short-date}.csv` file name. There is also a `historical.zip` file where data is archived.

There are three types of sensors:
- Temperature
- Humidity
- Rainfall

## Endpoints

Currently there are two possible endopints:

### Collect all of the measurements for one day, one sensor type, and one unit.
```
GET: /api/v1/devices/{deviceId}/data/{short-date}/{sensor-type}

example:
GET: /api/v1/devices/dockan/data/2019-01-11/temperature
```
- returns response `GetSensorDailyDataResponse`
```json
{
    "data": [
        {
            "date": "2019-01-11T00:00:00",
            "value": -0.77
        },
        {
            "date": "2019-01-11T00:00:05",
            "value": -0.78
        }
        // ...
    ]
}
```

### Collect all data points for one unit and one day.
- 
```
GET: /api/v1/devices/{deviceId}/data/{date-short-string}

example:
GET: /api/v1/devices/dockan/data/2019-01-11
```
- returns response `GetDeviceDailyDataResponse`
```json
{
"data": [
    {
        "date": "2019-01-11T00:00:00",
        "temperature": -0.77,
        "humidity": 9.18,
        "rainfall": 0
    },
    {
        "date": "2019-01-11T00:00:05",
        "temperature": -0.78,
        "humidity": 9.18,
        "rainfall": 0
    }
        // ...
    ]
}
```