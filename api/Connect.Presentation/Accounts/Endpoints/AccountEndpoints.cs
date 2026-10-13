using Connect.Application.Accounts.CreateAccount;
using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Presentation.Accounts.DTO;
using Connect.Presentation.Accounts.Mappers;
using Connect.Presentation.Common.Errors;
using Connect.Presentation.Common.Response;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Connect.Presentation.Accounts.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var accountGroup = endpoints.MapGroup("/api/accounts");

        accountGroup.MapPost("/", async Task<Results<
            Created<Response<CreateAccountResponseData>>,
            ValidationProblem,
            ProblemHttpResult>> (
                CreateAccountRequest request,
                IValidator<CreateAccountRequest> validator,
                ICommandHandler<CreateAccountCommand, Result<CreateAccountResult>> handler,
                CancellationToken cancellationToken) =>
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                return TypedResults.ValidationProblem(validation.ToDictionary());
            }

            var result = await handler.HandleAsync(
                new CreateAccountCommand(request.Email, request.Password),
                cancellationToken);

            if (result.IsSuccess)
            {
                return TypedResults.Created(
                    $"/api/accounts/{result.Value!.Id}",
                    CreateAccountResponseMapper.Map(result.Value));
            }

            return result.Error.ToProblemDetails();
        });

        return endpoints;
    }
}
