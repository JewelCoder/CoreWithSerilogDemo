using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreWithSerilogDemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Enters into {controller}/{action} method", "WeatherForecast", "Get");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet]
        public string TestDefaultOutTemplateWithThreadIdEnrich()
        {
            _logger.LogInformation("Enters into {controller}/{action} method", "WeatherForecast", "TestDefaultOutTemplateWithThreadIdEnrich");

            return "you have invoke TestDefaultOutTemplateWithThreadIdEnrich method successfully.";
        }

        [HttpGet]
        public string TestFilter()
        {
            _logger.LogInformation("Enters into {controller}/{action} method", "WeatherForecast", "TestFilter");

            // this will not be logged
            _logger.LogInformation("Count Value:{Count}", 20);

            // this wii be logged
            _logger.LogInformation("Count Value:{Count}", 5);

            return "you have invoke TestFilter method successfully.";
        }

        [HttpGet]
        public string TestMinimumLevel()
        {
            _logger.LogInformation("Enters into {controller}/{action} method", "WeatherForecast", "TestMinimumLevel");

            bool enabled = Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Debug);
            if (enabled)
                _logger.LogDebug("If LoggerConfiguration Sets minimum level to Information, then this message will not be logger");

            return "you have invoke TestMinimumLevel method successfully.";
        }

        [HttpGet]
        public string TestLogginLevelSwitch(string level)
        {
            _logger.LogInformation("Enters into {controller}/{action} method", "WeatherForecast", "TestLogginLevelSwitch");
            level = string.IsNullOrEmpty(level) ? string.Empty : level.ToUpper();
            switch (level)
            {
                case "VERBOSE":
                    CacheObjects.LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
                    break;
                case "DEBUG":
                    CacheObjects.LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Debug;
                    break;
                case "INFORMATION":
                    CacheObjects.LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;
                    break;
                case "WARNING":
                    CacheObjects.LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Warning;
                    break;
                case "ERROR":
                    CacheObjects.LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Error;
                    break;
                case "FATAL":
                    CacheObjects.LoggingLevelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Fatal;
                    break;
                default:
                    break;
            }

            _logger.LogTrace("This is a {level} message, current minimum level is {eventLevel}", "trace", CacheObjects.LoggingLevelSwitch.MinimumLevel.ToString());
            _logger.LogDebug("This is a {level} message, current minimum level is {eventLevel}", "debug", CacheObjects.LoggingLevelSwitch.MinimumLevel.ToString());
            _logger.LogInformation("This is a {level} message, current minimum level is {eventLevel}", "information", CacheObjects.LoggingLevelSwitch.MinimumLevel.ToString());
            _logger.LogWarning("This is a {level} message, current level minimum is {eventLevel}", "warning", CacheObjects.LoggingLevelSwitch.MinimumLevel.ToString());
            _logger.LogError("This is a {level} message, current level minimum is {eventLevel}", "error", CacheObjects.LoggingLevelSwitch.MinimumLevel.ToString());
            _logger.LogCritical("This is a {level} message, current minimum level is {eventLevel}", "critical", CacheObjects.LoggingLevelSwitch.MinimumLevel.ToString());

            return "you have invoke TestLogginLevelSwitch method successfully.";
        }

        public String TestSourceText()
        {
            _logger.LogInformation("Enters into {controller}/{action} method", "WeatherForecast", "TestSourceText");



            return "you have invoke TestSourceText method successfully.";
        }
    }
}
