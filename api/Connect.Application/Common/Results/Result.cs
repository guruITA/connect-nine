namespace Connect.Application.Common.Results;

public sealed record Result(bool IsSuccess, Error Error)
{
    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}
