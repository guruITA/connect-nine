namespace Connect.Presentation.Profiles.Validators;

// The id of a member page arrives as text, so "abc" and an unknown id both end in 404.
public static class MemberProfileIdValidator
{
    public static bool TryParse(string id, out Guid profileId)
    {
        return Guid.TryParse(id, out profileId);
    }
}
