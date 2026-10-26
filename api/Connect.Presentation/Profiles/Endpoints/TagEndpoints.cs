using Connect.Application.Common.Abstractions;
using Connect.Application.Common.Results;
using Connect.Application.Profiles.ListSuggestedTags;
using Connect.Presentation.Common.Errors;
using Connect.Presentation.Common.Response;
using Connect.Presentation.Profiles.DTO;
using Connect.Presentation.Profiles.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Connect.Presentation.Profiles.Endpoints;

public static class TagEndpoints
{
    public static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var tagGroup = endpoints.MapGroup("/api/tags").RequireAuthorization();

        tagGroup.MapGet("/suggested", async Task<Results<
            Ok<Response<IReadOnlyList<TagResponse>, SuggestedTagsMeta>>,
            ProblemHttpResult>> (
                string? page,
                string? pageSize,
                IQueryHandler<ListSuggestedTagsQuery, Result<SuggestedTagsResult>> handler,
                CancellationToken cancellationToken) =>
        {
            var problem = SuggestedTagsQueryValidator.Check(page, pageSize, out var pageNumber, out var size);
            if (problem is not null)
            {
                return Error.Validation(problem).ToProblemDetails();
            }

            var result = await handler.HandleAsync(new ListSuggestedTagsQuery(pageNumber, size), cancellationToken);
            if (result is not { IsSuccess: true, Value: { } value })
            {
                return result.Error.ToProblemDetails();
            }

            return TypedResults.Ok(new Response<IReadOnlyList<TagResponse>, SuggestedTagsMeta>(
                value.Tags.Select(tag => new TagResponse(tag.Id, tag.Name)).ToList(),
                new SuggestedTagsMeta(value.Page, value.PageSize, value.Total)));
        });

        return endpoints;
    }
}
