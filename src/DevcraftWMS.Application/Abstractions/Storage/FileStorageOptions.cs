namespace DevcraftWMS.Application.Abstractions.Storage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string Provider { get; init; } = "FileSystem";
    public string BasePath { get; init; } = string.Empty;
    public string? BaseUrl { get; init; }
    public string AsnAttachmentsPath { get; init; } = "asns";
    public bool StoreContentBase64 { get; init; }
    public long MaxFileSizeBytes { get; init; } = 10_000_000;
    public string[] AllowedContentTypes { get; init; } = Array.Empty<string>();
}
