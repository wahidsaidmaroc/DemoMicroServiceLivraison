using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ServiceNotification.WebAPI.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation error");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, new
            {
                title = "Validation Error",
                status = 400,
                errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found");
            await WriteErrorResponse(context, HttpStatusCode.NotFound, new
            {
                title = "Not Found",
                status = 404,
                detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Invalid operation");
            await WriteErrorResponse(context, HttpStatusCode.UnprocessableEntity, new
            {
                title = "Invalid Operation",
                status = 422,
                detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, new
            {
                title = "Internal Server Error",
                status = 500,
                detail = "An unexpected error occurred."
            });
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, object body)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
