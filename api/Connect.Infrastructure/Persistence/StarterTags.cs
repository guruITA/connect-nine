namespace Connect.Infrastructure.Persistence;

// The suggested tags a young person can tap (issue 36, task 36.1). Only skills and interests, in
// small letters, and nothing about health, faith, sexuality, politics or origin. A draft until the
// team approves it, see question 9 of issue 10 (#11).
internal static class StarterTags
{
    private static readonly DateTimeOffset AddedAt = new(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly string[] Names =
    [
        "coding", "cooking", "dance", "design", "drawing", "drone video", "football", "gaming",
        "gardening", "languages", "music", "photography", "reading", "running", "social media",
        "swimming", "theatre", "video editing", "volunteering", "writing"
    ];

    // Fixed ids and a fixed date, so a new migration never sees a change in the seed data.
    public static object[] All => Names
        .Select((name, index) => (object)new
        {
            Id = new Guid($"00000000-0000-4000-8000-0000000360{index + 1:D2}"),
            Name = name,
            IsSuggested = true,
            CreatedAt = AddedAt,
            UpdatedAt = AddedAt
        })
        .ToArray();
}
