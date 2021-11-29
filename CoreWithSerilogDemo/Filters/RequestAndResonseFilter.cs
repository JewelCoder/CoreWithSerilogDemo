using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CoreWithSerilogDemo
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequestAndResonseFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var diagnosticContext = context.HttpContext.RequestServices.GetService<IDiagnosticContext>();
            if (context.HttpContext.Request.Query.Any())
            {
                diagnosticContext.Set("QueryString", context.HttpContext.Request.Query);
            }
            var logger = context.HttpContext.RequestServices.GetService<ILogger>();
            logger.ForContext(context.Controller.GetType());
            logger.Information("{action} ends at {time:yyyy-MM-dd HH:mm}", context.ActionDescriptor.DisplayName, DateTime.Now);
        }
    }
}
