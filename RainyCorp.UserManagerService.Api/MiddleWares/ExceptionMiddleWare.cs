using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RainyCorp.UserManagerService.Api.MiddleWares
{
    public class ExceptionMiddleWare
    {
        private readonly ILogger<ExceptionMiddleWare> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong, encountered exception {ex}", ex);

                await context.Response.WriteAsync(new
                {
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Error occurred"
                }.ToString());
            }
        }
    }
}