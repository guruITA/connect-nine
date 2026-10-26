using System.Linq.Expressions;
using Connect.Domain.Users;

namespace Connect.Domain.ParentConsents;

// Who may see a profile that waits for a parent (rule 3 of #24). An expression, so a query
// can apply it before the projection instead of loading every profile first.
public static class ParentConsentVisibility
{
    public static Expression<Func<Profile, bool>> IsVisibleTo(Guid viewerAccountId)
    {
        return profile => profile.ParentConsent == null
            || profile.ParentConsent.GivenAt != null
            || profile.AccountId == viewerAccountId;
    }
}
