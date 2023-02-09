using System.Net;
using Newtonsoft.Json;

namespace static_sv.Exceptions
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse();
            
            if(exception is ErrorResponseException err)
            {
                context.Response.StatusCode = err.StatusCode;
                response = new ErrorResponse{
                    StatusCode=err.StatusCode,
                    Message=err.Description,
                    Errors=err.Errors
                };
            }else{
                response = new ErrorResponse
                {
                    StatusCode=statusCode,
                    Message = exception.Message
                };
            }

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseExceptionHandler(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}