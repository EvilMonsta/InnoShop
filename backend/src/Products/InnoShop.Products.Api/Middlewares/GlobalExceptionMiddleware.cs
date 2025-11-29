using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Products.Api.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = null };

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            await HandleAsync(ctx, ex);
        }
    }

    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var status = HttpStatusCode.InternalServerError;
        string title = "Internal Server Error";
        object? extensions = null;

        switch (ex)
        {
            case ValidationException fv:
                status = HttpStatusCode.BadRequest;
                title = "Validation failed";
                extensions = new
                {
                    errors = fv.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                };
                break;

            case UnauthorizedAccessException:
                status = HttpStatusCode.Unauthorized;
                title = "Unauthorized";
                break;

            case KeyNotFoundException:
                status = HttpStatusCode.NotFound;
                title = "Not Found";
                break;

            case OperationCanceledException:
                status = (HttpStatusCode)499;
                title = "Request cancelled";
                break;

            case InvalidOperationException ioe:
                status = HttpStatusCode.BadRequest;
                title = ioe.Message;
                break;

            default:
                _logger.LogError(ex, "Unhandled exception");
                break;
        }

        var pd = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{(int)status}",
            Title = title,
            Status = (int)status,
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };

        if (extensions is not null)
        {
            foreach (var prop in extensions.GetType().GetProperties())
            {
                pd.Extensions[prop.Name] = prop.GetValue(extensions);
            }
        }

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = pd.Status ?? (int)HttpStatusCode.InternalServerError;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(pd, JsonOpts));
    }
}
