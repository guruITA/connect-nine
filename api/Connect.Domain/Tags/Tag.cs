using Connect.Domain.Common;

namespace Connect.Domain.Tags;

// One row per tag name. Football and football are the same tag (the name is citext).
public sealed class Tag : Entity
{
    public required string Name { get; set; }

    public bool IsSuggested { get; set; }
}
