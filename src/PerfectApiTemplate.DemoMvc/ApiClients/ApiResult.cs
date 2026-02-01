namespace PerfectApiTemplate.DemoMvc.ApiClients;

public sealed class ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public static ApiResult<T> Success(T data, int statusCode) => new()
    {
        IsSuccess = true,
        Data = data,
        StatusCode = statusCode
    };

    public static ApiResult<T> Failure(string error, int statusCode) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };
}

