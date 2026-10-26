namespace Connect.Domain.Tags;

public static class TagRules
{
    public const int MaxLength = 30;

    public const int MaxPerProfile = 10;

    // Takes the spaces off both ends, turns every run of spaces into one space and makes it lower
    // case, so "  Drone   Video " and "drone video" are the same tag.
    public static string Normalize(string name)
    {
        return string.Join(' ', name.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .ToLowerInvariant();
    }
}
