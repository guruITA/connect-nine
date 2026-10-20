namespace Connect.Application.Common.Results;

public sealed record Result
{
    private Result(Error error)
    {
        Error = error;
    }

    public Error Error { get; }

    public bool IsSuccess => Error.Code == ErrorCode.None;

    public static Result Success() => new(Error.None);

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (error.Code == ErrorCode.None)
        {
            throw new ArgumentException("A failure must contain an error.", nameof(error));
        }

        return new(error);
    }
}
