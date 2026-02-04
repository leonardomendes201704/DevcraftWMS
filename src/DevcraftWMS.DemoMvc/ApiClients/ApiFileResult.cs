namespace DevcraftWMS.DemoMvc.ApiClients;

public sealed class ApiFileResult
{
    public bool IsSuccess { get; init; }
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = "application/octet-stream";
    public string FileName { get; init; } = "download";
    public string? Error { get; init; }
    public int StatusCode { get; init; }

    public static ApiFileResult Success(byte[] content, string contentType, string fileName, int statusCode) => new()
    {
        IsSuccess = true,
        Content = content,
        ContentType = contentType,
        FileName = fileName,
        StatusCode = statusCode
    };

    public static ApiFileResult Failure(string error, int statusCode) => new()
    {
        IsSuccess = false,
        Error = error,
        StatusCode = statusCode
    };
}
