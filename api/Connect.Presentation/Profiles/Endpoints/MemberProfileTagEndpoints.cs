using System.Security.Claims;
using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.AddProfileTag;
using Connect.Domain.Tags;
using Connect.Presentation.Common.Authentication;
using Connect.Presentation.Common.Errors;
using Connect.Presentation.Common.Response;
using Connect.Presentation.Profiles.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Connect.Presentation.Profiles.Endpoints;

public static class MemberProfileTagEndpoints
{
    public static IEndpointRouteBuilder MapMemberProfileTagEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var tagGroup = endpoints.MapGroup("/api/member-profiles/current/tags").RequireAuthorization();

        tagGroup.MapPost("/", async Task<Results<
            Created<Response<TagResponse>>,
            Ok<Response<TagResponse>>,
            ValidationProblem,
            ProblemHttpResult>> (
                AddTagRequest request,
                ClaimsPrincipal user,
                IValidator<AddTagRequest> validator,
                ICommandHandler<AddProfileTagCommand, Result<AddProfileTagResult>> handler,
                CancellationToken cancellationToken) =>
        {
            var accountId = user.GetAccountId();
            if (accountId is null)
            {
                return Error.Unauthorized("Log in to see this page").ToProblemDetails();
            }

            var normalised = request with { Name = TagRules.Normalize(request.Name ?? string.Empty) };
            var validation = await validator.ValidateAsync(normalised, cancellationToken);
            if (!validation.IsValid)
            {
                return TypedResults.ValidationProblem(validation.ToDictionary());
            }

            var result = await handler.HandleAsync(
                new AddProfileTagCommand(accountId.Value, normalised.Name!),
                cancellationToken);
            if (!result.IsSuccess)
            {
                return result.Error.ToProblemDetails();
            }

            var tag = new Response<TagResponse>(new TagResponse(result.Value!.Tag.Id, result.Value.Tag.Name));
            return result.Value.Added
                ? TypedResults.Created("/api/member-profiles/current", tag)
                : TypedResults.Ok(tag);
        });

        return endpoints;
    }
}
