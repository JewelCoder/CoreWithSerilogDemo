using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWithSerilogDemo.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;

namespace CoreWithSerilogDemo
{
    public class Program
    {
        // 静态方式使用Serilog
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                         .WriteTo.Console()
                         .CreateLogger();
            try
            {
                Log.Logger.Information("Application Starts");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Fatal Error：{@error}", ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }


        // 注入方式使用Serilog
        public static IHostBuilder CreateHostBuilder(string[] args) =>
                                    Host.CreateDefaultBuilder(args)
                                    .UseSerilog((context, services, configureation) =>
                                    {
                                        CacheObjects.LoggingLevelSwitch = new Serilog.Core.LoggingLevelSwitch();
                                        CacheObjects.LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;
                                        configureation.Enrich.With(new ThreadIdEnricher())
                                                       .Enrich.FromLogContext()
                                                       .MinimumLevel.ControlledBy(CacheObjects.LoggingLevelSwitch)
                                                       .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ThreadId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                                                       .Filter.ByExcluding(Matching.WithProperty<int>("Count", o => o > 10))
                                                       .WriteTo.Logger(loggerConfig =>
                                                       {
                                                           loggerConfig.Filter.ByIncludingOnly(Matching.FromSource<WeatherForecastController>())
                                                                        .WriteTo.File("app.log");
                                                       });
                                    }, false)
                                    .ConfigureWebHostDefaults(webBuilder =>
                                    {
                                        webBuilder.UseStartup<Startup>();
                                    });
    }
}
