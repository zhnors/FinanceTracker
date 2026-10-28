using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinanceTracker.Api.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            OperationCanceledException => 499,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = status;

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if(status == StatusCodes.Status500InternalServerError) _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);

        if(status == StatusCodes.Status400BadRequest) _logger.LogWarning("Bad request: {Message}. TraceId: {TraceId}", exception.Message, traceId);

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = status == 400 ? "Invalid request" : "Server error",
                Detail = status == 400 ? exception.Message : "An unexpected error occurred."
            }
        });
    }
}