using Microsoft.AspNetCore.Http;
using ResultHandler;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SharedModule.Middlewares
{
    public class ErrorHandlerMiddleware : IMiddleware
    {
        private readonly ILogger _logger;
        public ErrorHandlerMiddleware(ILogger logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException error)
            {
                _logger.Information($"Invalid User Input: {error}");
                await handleValidationAsync(context, "", "");
            }
            catch (Exception error)
            {
                _logger.Error($"Internal Server Error: {error}");
                await handleExceptionAsync(context);
            }
        }

        private Task handleExceptionAsync(HttpContext context)
        {
            return context.Response.WriteAsync(JsonSerializer.Serialize(new CommitResult
            {
                ResultType = ResultType.InternalServerError,
                ErrorMessage = "Internal Server Error",
                ErrorCode = "X0000"
            }));
        }
        private Task handleValidationAsync(HttpContext context, string errorMessage, string errorCode)
        {
            return context.Response.WriteAsync(JsonSerializer.Serialize(new CommitResult
            {
                ResultType = ResultType.InvalidValidation,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            }));
        }
    }
}
