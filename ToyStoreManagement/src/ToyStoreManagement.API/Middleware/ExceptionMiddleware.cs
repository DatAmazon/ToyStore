using System.Text.Json;
using ToyStoreManagement.Application.DTOs.Common;

namespace ToyStoreManagement.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred. Message: {Message}",
                ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            UnauthorizedAccessException =>
                StatusCodes.Status401Unauthorized,

            KeyNotFoundException =>
                StatusCodes.Status404NotFound,

            ArgumentException =>
                StatusCodes.Status400BadRequest,

            _ =>
                StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.FailureResponse(
            _environment.IsDevelopment() ? exception.Message : GetDefaultMessage(statusCode),
            _environment.IsDevelopment() ? exception.StackTrace : null
        );

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }

    private static string GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad request.",
            StatusCodes.Status401Unauthorized => "Unauthorized.",
            StatusCodes.Status404NotFound => "Resource not found.",
            _ => "Internal server error."
        };
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}