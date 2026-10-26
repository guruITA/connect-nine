using System.Security.Claims;
using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.CreateProfile;
using Connect.Application.Profiles.GetCurrentProfile;
using Connect.Domain.Users;
using Connect.Presentation.Common.Authentication;
using Connect.Presentation.Common.Errors;
using Connect.Presentation.Common.Response;
using Connect.Presentation.Profiles.DTO;
using Connect.Presentation.Profiles.Mappers;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Connect.Presentation.Profiles.Endpoints;

public static class MemberProfileEndpoints
{
    public static IEndpointRouteBuilder MapMemberProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var memberProfileGroup = endpoints.MapGroup("/api/member-profiles")
            .RequireAuthorization();

        memberProfileGroup.MapGet("/current", async Task<Results<
            Ok<Response<MemberProfileResponseData>>,
            ProblemHttpResult>> (
                ClaimsPrincipal user,
                IQueryHandler<GetCurrentProfileQuery, Result<CurrentProfileResult>> handler,
                CancellationToken cancellationToken) =>
        {
            var accountId = user.GetAccountId();
            if (accountId is null)
            {
                return Error.Unauthorized("Log in to see this page").ToProblemDetails();
            }

            var result = await handler.HandleAsync(
                new GetCurrentProfileQuery(accountId.Value),
                cancellationToken);

            return result.IsSuccess
                ? TypedResults.Ok(MemberProfileResponseMapper.Map(result.Value!))
                : result.Error.ToProblemDetails();
        });

        memberProfileGroup.MapPost("/", async Task<Results<
            Created<Response<CreateMemberProfileResponseData>>,
            ValidationProblem,
            ProblemHttpResult>> (
                CreateMemberProfileRequest request,
                ClaimsPrincipal user,
                IValidator<CreateMemberProfileRequest> validator,
                ICommandHandler<CreateProfileCommand, Result<CreateProfileResult>> handler,
                CancellationToken cancellationToken) =>
        {
            var accountId = user.GetAccountId();
            if (accountId is null)
            {
                return Error.Unauthorized("Log in first to save your profile").ToProblemDetails();
            }

            var trimmed = Trim(request);
            var validation = await validator.ValidateAsync(trimmed, cancellationToken);
            if (!validation.IsValid)
            {
                return TypedResults.ValidationProblem(validation.ToDictionary());
            }

            // The validator above refuses an empty first name, age band or region.
            if (trimmed is not { FirstName: { } firstName, AgeBand: { } ageBand, Region: { } region })
            {
                throw new InvalidOperationException("The validator let an empty first name, age band or region through.");
            }

            var result = await handler.HandleAsync(
                new CreateProfileCommand(
                    accountId.Value,
                    firstName,
                    Enum.Parse<AgeBand>(ageBand),
                    region,
                    trimmed.Bio),
                cancellationToken);

            if (result is { IsSuccess: true, Value: { } created })
            {
                return TypedResults.Created(
                    $"/api/member-profiles/{created.Id}",
                    CreateMemberProfileResponseMapper.Map(created));
            }

            return result.Error.ToProblemDetails();
        });

        return endpoints;
    }

    private static CreateMemberProfileRequest Trim(CreateMemberProfileRequest request)
    {
        var bio = request.Bio?.Trim();

        return request with
        {
            FirstName = request.FirstName?.Trim(),
            Region = request.Region?.Trim(),
            Bio = string.IsNullOrEmpty(bio) ? null : bio
        };
    }
}
