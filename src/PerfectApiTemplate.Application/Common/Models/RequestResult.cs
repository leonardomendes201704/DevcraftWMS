namespace PerfectApiTemplate.Application.Common.Models;

public sealed class RequestResult<T>
{
    private RequestResult(bool isSuccess, T? value, string? errorCode, string? errorMessage, IReadOnlyDictionary<string, string[]>? validationErrors)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    public static RequestResult<T> Success(T value) => new(true, value, null, null, null);

    public static RequestResult<T> Failure(string errorCode, string errorMessage) =>
        new(false, default, errorCode, errorMessage, null);

    public static RequestResult<T> ValidationFailure(IReadOnlyDictionary<string, string[]> errors) =>
        new(false, default, "validation_error", "One or more validation errors occurred.", errors);
}

