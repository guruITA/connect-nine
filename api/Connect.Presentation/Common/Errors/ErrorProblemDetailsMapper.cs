using Connect.Application.Common.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Connect.Presentation.Common.Errors;

public static class ErrorProblemDetailsMapper
{
    public static ProblemHttpResult ToProblemDetails(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var (statusCode, title) = error.Code switch
        {
            ErrorCode.Validation => (StatusCodes.Status400BadRequest, error.Description),
            ErrorCode.Unauthorized => (StatusCodes.Status401Unauthorized, error.Description),
            ErrorCode.NotFound => (StatusCodes.Status404NotFound, error.Description),
            ErrorCode.Conflict => (StatusCodes.Status409Conflict, error.Description),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        return TypedResults.Problem(statusCode: statusCode, title: title);
    }
}
