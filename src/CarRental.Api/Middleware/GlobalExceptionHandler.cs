using CarRental.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = MapException(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "{Title} while processing {Method} {Path}", title, httpContext.Request.Method, httpContext.Request.Path);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception) => exception switch
    {
        NotFoundException notFound => (StatusCodes.Status404NotFound, "Resource not found", notFound.Message),
        ConflictException conflict => (StatusCodes.Status409Conflict, "Conflict", conflict.Message),
        BusinessRuleException businessRule => (StatusCodes.Status400BadRequest, "Business rule violation", businessRule.Message),
        ValidationException validation => (StatusCodes.Status400BadRequest, "Validation failed", validation.Message),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", "Please contact support if the problem persists.")
    };
}
