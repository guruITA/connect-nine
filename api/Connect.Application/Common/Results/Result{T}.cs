namespace Connect.Application.Common.Results;

public sealed record Result<T>
{
    private Result(T? value, Error error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }

    public Error Error { get; }

    public bool IsSuccess => Error.Code == ErrorCode.None;

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(value, Error.None);
    }

    public static Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (error.Code == ErrorCode.None)
        {
            throw new ArgumentException("A failure must contain an error.", nameof(error));
        }

        return new(default, error);
    }
}
