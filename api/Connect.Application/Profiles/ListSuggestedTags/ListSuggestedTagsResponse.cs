namespace Connect.Application.Profiles.ListSuggestedTags;

public sealed record SuggestedTagsResult(IReadOnlyList<TagResult> Tags, int Page, int PageSize, int Total);
