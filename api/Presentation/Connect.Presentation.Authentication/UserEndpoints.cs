using Connect.Application.Authentication.Users;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Connect.Presentation.Authentication;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/users", CreateUserAsync);
        return endpoints;
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        IValidator<CreateUserRequest> validator,
        UserService userService,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            return Results.ValidationProblem(errors);
        }

        try
        {
            var result = await userService.CreateUserAsync(
                new CreateUserCommand(request.Email, request.Password, request.DisplayName),
                cancellationToken);

            return Results.Created(
                $"/api/users/{result.Id}",
                new { data = new CreateUserResponse(result.Id, result.Email, result.DisplayName, result.CreatedAt), meta = new { } });
        }
        catch (DuplicateUserException)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "User already exists",
                detail: "An account with this email address already exists.");
        }
    }

}
