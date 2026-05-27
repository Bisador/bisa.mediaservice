using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            logger.LogWarning(ex, ex.Message);
            await WriteProblemDetails(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            await WriteProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = message,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problem));
    }
}