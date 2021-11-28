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
                                        CacheObjects.LoggingLevelSwitch = new Serilog.Core.LoggingLevelSwitch()
                                        {
                                            MinimumLevel = Serilog.Events.LogEventLevel.Information
                                        };
                                        configureation.Enrich.FromLogContext()
                                                       .Enrich.AtLevel(Serilog.Events.LogEventLevel.Warning,
                                                                       // we can use Enrich.WithThreadId(),it's in the Serilog.Enrichers.Thread package
                                                                       enrich => enrich.With(new ThreadIdEnricher()))
                                                       .Enrich.When(
                                                        // Here, we can generate complicate rules.
                                                        _lvl => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production",
                                                        enrich => enrich.WithProperty("Version", typeof(Program).Assembly.GetName().Version.ToString())
                                                        )
                                                       .MinimumLevel.ControlledBy(CacheObjects.LoggingLevelSwitch)
                                                       // this makes logs from the namespace starts with "Microsoft.AspNetCore" be not recorded
                                                       .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Error)
                                                       .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {ThreadId} {Version} [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                                                       .Filter.ByExcluding(Matching.WithProperty<int>("Count", o => o > 10))
                                                       .WriteTo.Logger(loggerConfig =>
                                                       {
                                                           loggerConfig.Filter.ByIncludingOnly(Matching.FromSource<WeatherForecastController>())
                                                                        // RenderedCompactJsonFormatter will record every property and the seriablized object value
                                                                        // .WriteTo.File(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(), "app.log");
                                                                        .WriteTo.File("app.log");
                                                       });
                                    }, false)
                                    .ConfigureWebHostDefaults(webBuilder =>
                                    {
                                        webBuilder.UseStartup<Startup>();
                                    });
    }
}
