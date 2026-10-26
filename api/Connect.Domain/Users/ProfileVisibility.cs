namespace Connect.Domain.Users;

// What another member may see of a profile. Follows the role table of #28
// (docs/security/roleTable.md): never the region of a young person of 15 to 17. Change this
// class and task 14.3 (#343) together when that table says otherwise.
public static class ProfileVisibility
{
    public static bool ShowsRegion(AgeBand ageBand)
    {
        return ageBand != AgeBand.Adolescent;
    }

    public static bool ShowsBio(AgeBand ageBand)
    {
        return true;
    }

    // Tags follow the same rule as the short text (task 36c.2).
    public static bool ShowsTags(AgeBand ageBand)
    {
        return ShowsBio(ageBand);
    }
}
