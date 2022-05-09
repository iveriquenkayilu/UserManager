using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using UserManagerService.Shared.Exceptions;
using UserManagerService.Shared.Models;

namespace UserManagerService.Api.MiddleWares
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
            catch (CustomException ex)
            {
                _logger.LogError(ex.Message, ex);
                await context.Response.WriteAsJsonAsync(
                    ResponseModel.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong, encountered exception {ex}", ex);

                await context.Response.WriteAsJsonAsync(new
                {
                    ResponseCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Error occurred"
                });
            }
        }
    }
}