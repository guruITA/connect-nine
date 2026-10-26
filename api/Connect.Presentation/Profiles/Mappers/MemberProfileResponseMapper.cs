using Connect.Application.Profiles.GetCurrentProfile;
using Connect.Presentation.Common.Response;
using Connect.Presentation.Profiles.DTO;

namespace Connect.Presentation.Profiles.Mappers;

public static class MemberProfileResponseMapper
{
    public static Response<MemberProfileResponseData> Map(CurrentProfileResult result)
    {
        return new(new MemberProfileResponseData(
            result.FirstName,
            result.AgeBand.ToString(),
            result.Region,
            result.Bio));
    }
}
