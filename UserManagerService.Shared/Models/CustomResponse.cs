using Microsoft.AspNetCore.Mvc;

namespace UserManagerService.Shared.Models
{
    public class CustomResponse
    {
        public string Message { get; set; }
        public bool Error { get; set; }

        public static BadRequestObjectResult Fail<T>(string message, T data = default) => new BadRequestObjectResult(new Response<T>(data, message, true));

        public static OkObjectResult Success<T>(string message, T data = default) => new OkObjectResult(new Response<T>(data, message, false));

        public static BadRequestObjectResult Fail(string message) => new BadRequestObjectResult(new CustomResponse(message, true));

        public static OkObjectResult Success(string message) => new OkObjectResult(new CustomResponse(message, false));

        public CustomResponse(string message, bool error)
        {
            Message = message;
            Error = error;
        }
    }

    public class Response<T> : CustomResponse
    {
        public T Data { get; set; }

        public Response(T data, string message, bool error) : base(message, error)
        {
            Data = data;
        }
    }
}

