namespace Connect.Application.Profiles.GetPublicProfile;

public sealed record GetPublicProfileQuery(Guid ProfileId, Guid ViewerAccountId);
