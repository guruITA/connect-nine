using Connect.Application.Profiles.CreateProfile;
using Connect.Presentation.Common.Response;
using Connect.Presentation.Profiles.DTO;

namespace Connect.Presentation.Profiles.Mappers;

public static class CreateMemberProfileResponseMapper
{
    public static Response<CreateMemberProfileResponseData> Map(CreateProfileResult result)
    {
        return new(new CreateMemberProfileResponseData(result.Id));
    }
}
