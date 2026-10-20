namespace Connect.Application.Common.Results;

public sealed record Error(ErrorCode Code, string Description)
{
    public static Error None => new(ErrorCode.None, "");

    public static Error Validation(string description) =>
        new(ErrorCode.Validation, description);

    public static Error NotFound(string description) =>
        new(ErrorCode.NotFound, description);

    public static Error Conflict(string description) =>
        new(ErrorCode.Conflict, description);

    public static Error Unauthorized(string description) =>
        new(ErrorCode.Unauthorized, description);

    public static Error Failure =>
        new(ErrorCode.Failure, "Internal server error");
}
