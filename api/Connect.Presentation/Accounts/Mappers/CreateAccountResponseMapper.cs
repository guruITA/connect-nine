using Connect.Application.Accounts.CreateAccount;
using Connect.Presentation.Accounts.DTO;
using Connect.Presentation.Common.Response;

namespace Connect.Presentation.Accounts.Mappers;

public static class CreateAccountResponseMapper
{
    public static Response<CreateAccountResponseData> Map(CreateAccountResult response)
    {
        return new(new CreateAccountResponseData(response.Id, response.Email));
    }
}
