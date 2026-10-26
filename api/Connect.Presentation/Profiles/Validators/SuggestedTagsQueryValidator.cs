namespace Connect.Presentation.Profiles.Validators;

// page and pageSize arrive as text, so a wrong value gives a plain 400 and never a server error.
public static class SuggestedTagsQueryValidator
{
    public const int MaxPageSize = 50;

    public static string? Check(string? page, string? pageSize, out int pageNumber, out int size)
    {
        pageNumber = 1;
        size = MaxPageSize;

        if (page is not null && (!int.TryParse(page, out pageNumber) || pageNumber < 1))
        {
            return "Use a page number of 1 or more";
        }

        if (pageSize is not null && (!int.TryParse(pageSize, out size) || size < 1 || size > MaxPageSize))
        {
            return "Use a page size from 1 to 50";
        }

        return null;
    }
}
