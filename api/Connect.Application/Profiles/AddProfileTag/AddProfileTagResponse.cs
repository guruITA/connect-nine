namespace Connect.Application.Profiles.AddProfileTag;

// Added is false when the profile already had this tag, which changes nothing.
public sealed record AddProfileTagResult(TagResult Tag, bool Added);
