using Connect.Application.Common.Results;
using Connect.Presentation.Common.Errors;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Connect.Presentation.Tests.Common.Errors;

public sealed class ErrorProblemDetailsMapperTests
{
    [TestCase(ErrorCode.Validation, StatusCodes.Status400BadRequest)]
    [TestCase(ErrorCode.Unauthorized, StatusCodes.Status401Unauthorized)]
    [TestCase(ErrorCode.NotFound, StatusCodes.Status404NotFound)]
    [TestCase(ErrorCode.Conflict, StatusCodes.Status409Conflict)]
    [TestCase(ErrorCode.Failure, StatusCodes.Status500InternalServerError)]
    public void ToProblemDetails_MapsErrorCodeToStatus(
        ErrorCode errorCode,
        int expectedStatusCode)
    {
        var result = new Error(errorCode, "already exists");

        var problem = result.ToProblemDetails();

        Assert.That(problem.StatusCode, Is.EqualTo(expectedStatusCode));
    }

    [Test]
    public void ToProblemDetails_UsesGenericTitleForFailure()
    {
        var problem = Error.Failure.ToProblemDetails();

        Assert.That(problem.ProblemDetails.Title, Is.EqualTo("An unexpected error occurred."));
    }
}
