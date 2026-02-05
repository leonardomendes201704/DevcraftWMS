namespace DevcraftWMS.Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<FileStorageResult> SaveAsync(FileSaveRequest request, CancellationToken cancellationToken = default);
    Task<FileReadResult?> ReadAsync(FileReadRequest request, CancellationToken cancellationToken = default);
}

public sealed record FileSaveRequest(
    string FileName,
    string ContentType,
    byte[] Content,
    string Category);

public sealed record FileReadRequest(
    string Provider,
    string? StorageKey,
    string? ContentBase64,
    string FileName,
    string ContentType);

public sealed record FileStorageResult(
    string Provider,
    string? StorageKey,
    string? StorageUrl,
    string? ContentBase64,
    string ContentHash,
    long SizeBytes);

public sealed record FileReadResult(
    byte[] Content,
    string ContentType,
    string FileName);
