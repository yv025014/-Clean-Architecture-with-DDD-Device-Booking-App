using DBA.Application.Common.Exceptions;
using DBA.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DBA.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, detail) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            DomainException => (StatusCodes.Status400BadRequest, exception.Message),
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            _ => (StatusCodes.Status500InternalServerError, "發生未預期的錯誤。")
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Detail = detail,
            },
            cancellationToken);

        return true;
    }
}
