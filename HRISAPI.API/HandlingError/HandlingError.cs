using HRISAPI.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace HRISAPI.API.HandlingError
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code;
            string result;
            Console.WriteLine(exception);

            switch (exception)
            {
                case BadRequestException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;
                case NotFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;
                case InvalidOperationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { error = exception.Message });
                    break;
                // Add more custom exceptions and HTTP status codes as needed

                default:
                    code = HttpStatusCode.InternalServerError;
                    result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }

}

