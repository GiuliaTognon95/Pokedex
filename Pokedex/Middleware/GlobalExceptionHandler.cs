namespace Pokedex;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

public class GlobalExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error occurred"
        };

        if (exception is InvalidOperationException ioe)
        {
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Title = "Sorry! I'm not able to find it :(";
            problemDetails.Detail = ioe.Message;
        }
        else
        {
            problemDetails.Detail = exception.Message;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}