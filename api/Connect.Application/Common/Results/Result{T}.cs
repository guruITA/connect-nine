namespace Connect.Application.Common.Results;

public sealed record Result<T>(T? Value, Error Error)
{
    public bool IsSuccess => Error == Error.None;

    public static Result<T> Success(T value) => new(value, Error.None);

    public static Result<T> Failure(Error error) => new(default, error);
}
