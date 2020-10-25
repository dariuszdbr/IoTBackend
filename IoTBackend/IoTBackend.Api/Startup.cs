using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IoTBackend.Infrastructure;
using IoTBackend.Infrastructure.Features.Devices.Handlers.GetSensorTypeDailyData;
using IoTBackend.Infrastructure.Features.Devices.Interfaces;
using IoTBackend.Infrastructure.Features.Devices.Parsers;
using IoTBackend.Infrastructure.Features.Devices.Providers;
using IoTBackend.Infrastructure.Features.Devices.Readers;
using IoTBackend.Infrastructure.Shared.Configurations;
using IoTBackend.Infrastructure.Shared.Converters;
using IoTBackend.Infrastructure.Shared.Providers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IoTBackend.Api
{
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
            services.Configure<BlobConfiguration>(Configuration.GetSection("BlobConfiguration"));


            // Todo cleanup registered services add singletons, scoped etc

            services.AddTransient<ISensorTypeConverter, SensorTypeConverter>();
            services.AddTransient<IZipArchiveProvider, ZipArchiveProvider>();
            services.AddTransient<IStreamReaderProvider, StreamReaderProvider>();
            services.AddTransient<IStreamParser, StreamParser>();
            services.AddTransient<IBlobPathProvider, BlobPathProvider>();
            services.AddTransient<IBlobClientProvider, BlobClientProvider>();
            services.AddTransient<ISensorTypeConverter, SensorTypeConverter>();

            services.AddTransient<ISensorDataParser, HumidityDataParser>();
            services.AddTransient<ISensorDataParser, TemperatureDataParser>();
            services.AddTransient<ISensorDataParser, RainfallDataParser>();
            services.AddTransient<ISensorDataParserProvider, SensorDataParserProvider>();

            services.AddTransient<IBlobReader, BlobFileReader>();
            services.AddTransient<IBlobReader, BlobArchiveReader>();

            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(GetSensorTypeDailyDataRequestHandler).Assembly);
           
            services.AddControllers();
            services.AddApiVersioning();
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
