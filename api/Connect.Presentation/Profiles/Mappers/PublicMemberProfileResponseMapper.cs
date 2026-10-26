using Connect.Application.Profiles.GetPublicProfile;
using Connect.Presentation.Common.Response;
using Connect.Presentation.Profiles.DTO;

namespace Connect.Presentation.Profiles.Mappers;

public static class PublicMemberProfileResponseMapper
{
    public static Response<PublicMemberProfileResponseData> Map(GetPublicProfileResponse result)
    {
        return new(new PublicMemberProfileResponseData(
            result.FirstName,
            result.AgeBand.ToString(),
            result.Region,
            result.Bio,
            result.Tags));
    }
}
