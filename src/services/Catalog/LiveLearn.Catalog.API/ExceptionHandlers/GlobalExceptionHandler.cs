using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Catalog.API.ExceptionHandlers;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Course.Revision is a concurrency token: a concurrent structural change won the race; the client can retry.
        if (exception is DbUpdateConcurrencyException)
        {
            logger.LogWarning(exception, "Concurrent modification detected");

            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Title = "Concurrent modification",
                    Detail = "The resource was modified by another request. Reload it and retry.",
                    Status = StatusCodes.Status409Conflict,
                    Extensions = { ["traceId"] = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier }
                }
            });
        }

        logger.LogError(exception, "Unhandled exception occurred");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred",
                Detail = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Extensions = { ["traceId"] = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier }
            }
        });
    }
}
