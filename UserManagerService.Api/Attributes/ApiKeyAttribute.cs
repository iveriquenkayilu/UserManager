using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserManagerService.Shared.Settings;

namespace UserManagerService.Api.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ApiKeySettings apiKeySettings = new();
            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiKeyAttribute>>();
            appSettings.GetSection("ApiKeySettings").Bind(apiKeySettings);

            var header = context.HttpContext.Request.Headers[apiKeySettings.Name].ToString();
            if (string.IsNullOrEmpty(header))
                logger.LogInformation($"Couldn't get header value with key {apiKeySettings.Name}");

            if (!context.HttpContext.Request.Headers.TryGetValue(apiKeySettings.Name, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key was not provided"
                };
                logger.LogInformation("Api Key was not provided");
                return;
            }

            if (!apiKeySettings.Value.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key is not valid"
                };
                logger.LogInformation("Api Key is not valid");
                return;
            }

            await next();
        }
    }
}
