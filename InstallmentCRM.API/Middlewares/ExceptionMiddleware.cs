using System.Net;
using System.Text.Json;
using InstallmentCRM.Application.Common.Exceptions;

namespace InstallmentCRM.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex, _logger);
        }
    }

    private static async Task HandleException(
        HttpContext context,
        Exception exception,
        ILogger logger)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,

            NotFoundException => StatusCodes.Status404NotFound,

            ConflictException => StatusCodes.Status409Conflict,

            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

            _ => StatusCodes.Status500InternalServerError
        };

        // Только неожиданные (5xx) ошибки логируем как Error - остальное штатное поведение API
        if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }

        object response = exception is ValidationException validationException
            ? new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Validation failed.",
                Errors = validationException.Errors
            }
            : new
            {
                StatusCode = context.Response.StatusCode,
                Message = context.Response.StatusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message
            };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
