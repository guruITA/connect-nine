using System.Security.Claims;

namespace Connect.Presentation.Common.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetAccountId(this ClaimsPrincipal user)
    {
        return Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId)
            ? accountId
            : null;
    }
}
