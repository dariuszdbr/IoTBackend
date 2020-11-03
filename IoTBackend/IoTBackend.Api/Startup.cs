using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using IoTBackend.Infrastructure.Core.Converters;
using IoTBackend.Infrastructure.Core.Models.Configurations;
using IoTBackend.Infrastructure.Core.Providers;
using IoTBackend.Infrastructure.Features.Devices.GetDeviceDailyData;
using IoTBackend.Infrastructure.Features.Devices.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Shared.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Providers;
using IoTBackend.Infrastructure.Features.Devices.Shared.Readers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoTBackend.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<BlobConfiguration>(Configuration.GetSection(nameof(BlobConfiguration)));
            services.AddTransient<ISensorTypeConverter, SensorTypeConverter>();
            services.AddTransient<IZipArchiveProvider, ZipArchiveProvider>();
            services.AddTransient<IStreamReaderProvider, StreamReaderProvider>();
            services.AddTransient<IStreamParser, StreamParser>();
            services.AddTransient<IBlobPathProvider, BlobPathProvider>();
            services.AddSingleton<IBlobClientProvider, BlobClientProvider>();
            services.AddTransient<ISensorTypeConverter, SensorTypeConverter>();
            services.AddTransient<ISensorDataParser, HumiditySensorDailyDataPointParser>();
            services.AddTransient<ISensorDataParser, TemperatureSensorDailyDataPointParser>();
            services.AddTransient<ISensorDataParser, RainfallSensorDailyDataPointParser>();
            services.AddTransient<ISensorDataParserProvider, SensorDataParserProvider>();
            services.AddTransient<BlobFileReader>();
            services.AddTransient<BlobArchiveReader>();
            services.AddTransient<IBlobReader>(service => new BlobReader(new []
            {
                (IBlobReader)service.GetService<BlobFileReader>(),
                (IBlobReader)service.GetService<BlobArchiveReader>()
            }));
            services.AddMediatR(
                Assembly.GetExecutingAssembly(), 
                typeof(GetSensorDailyDataRequestHandler).Assembly, 
                typeof(GetDeviceDailyDataRequestHandler).Assembly);
            services.AddControllers();
            services.AddApiVersioning();

            var cultureInfo = new CultureInfo(Configuration.GetSection("CultureConfiguration").Value);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
