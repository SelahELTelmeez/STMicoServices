using IdentityDomain.Enums;
using MediatR;

namespace IdentityDomain.DTO.Shared;
public record CommitResult(ResultType ResultType, string? ErrorCode = default, string? ErrorMessage = default, bool IsSuccess = true) : IRequest<CommitResult>;
public record CommitResult<TValue>(TValue? Value, ResultType ResultType, string? ErrorCode = default, string? ErrorMessage = default, bool IsSuccess = true) : CommitResult(ResultType, ErrorCode, ErrorMessage, IsSuccess), IRequest<CommitResult<TValue>>;

public static class CommitResultGenerator
{
    public static CommitResult NotFoundResult => new CommitResult(ResultType.NotFound, "ErrorCode", "ErrorMessage", false);
    public static CommitResult ExceptionResult => new CommitResult(ResultType.Exception, "ErrorCode", "ErrorMessage", false);
    public static CommitResult OkResult => new CommitResult(ResultType.Ok);
}
public static class CommitResultGenerator<TValue>
{
    public static CommitResult<TValue> NotFoundResult => new CommitResult<TValue>(default, ResultType.NotFound, "ErrorCode", "ErrorMessage", false);
    public static CommitResult<TValue> ExceptionResult => new CommitResult<TValue>(default, ResultType.Exception, "ErrorCode", "ErrorMessage", false);
    public static CommitResult<TValue> OkResult(TValue Value) => new CommitResult<TValue>(Value, ResultType.Ok);
}