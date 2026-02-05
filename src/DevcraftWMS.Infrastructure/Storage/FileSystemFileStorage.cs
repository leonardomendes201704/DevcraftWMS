using System.Security.Cryptography;
using DevcraftWMS.Application.Abstractions.Storage;

namespace DevcraftWMS.Infrastructure.Storage;

public sealed class FileSystemFileStorage : IFileStorage
{
    private readonly FileStorageOptions _options;

    public FileSystemFileStorage(FileStorageOptions options)
    {
        _options = options;
    }

    public async Task<FileStorageResult> SaveAsync(FileSaveRequest request, CancellationToken cancellationToken = default)
    {
        var provider = string.IsNullOrWhiteSpace(_options.Provider) ? "FileSystem" : _options.Provider;
        var hash = ComputeHash(request.Content);

        if (string.Equals(provider, "Database", StringComparison.OrdinalIgnoreCase) ||
            _options.StoreContentBase64)
        {
            var base64 = Convert.ToBase64String(request.Content);
            return new FileStorageResult(provider, null, null, base64, hash, request.Content.LongLength);
        }

        var basePath = _options.BasePath;
        if (string.IsNullOrWhiteSpace(basePath))
        {
            throw new InvalidOperationException("FileStorage:BasePath is required for FileSystem provider.");
        }

        var category = string.IsNullOrWhiteSpace(request.Category) ? "files" : request.Category;
        var fileName = SanitizeFileName(request.FileName);
        var fileKey = $"{category}/{Guid.NewGuid():N}_{fileName}";
        var fullPath = Path.Combine(basePath, fileKey.Replace('/', Path.DirectorySeparatorChar));

        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(fullPath, request.Content, cancellationToken);

        var storageUrl = string.IsNullOrWhiteSpace(_options.BaseUrl)
            ? null
            : $"{_options.BaseUrl.TrimEnd('/')}/{fileKey}";

        return new FileStorageResult(provider, fileKey, storageUrl, null, hash, request.Content.LongLength);
    }

    public async Task<FileReadResult?> ReadAsync(FileReadRequest request, CancellationToken cancellationToken = default)
    {
        if (string.Equals(request.Provider, "Database", StringComparison.OrdinalIgnoreCase) ||
            !string.IsNullOrWhiteSpace(request.ContentBase64))
        {
            if (string.IsNullOrWhiteSpace(request.ContentBase64))
            {
                return null;
            }

            var contentBytes = Convert.FromBase64String(request.ContentBase64);
            return new FileReadResult(contentBytes, request.ContentType, request.FileName);
        }

        if (string.IsNullOrWhiteSpace(request.StorageKey))
        {
            return null;
        }

        var basePath = _options.BasePath;
        if (string.IsNullOrWhiteSpace(basePath))
        {
            return null;
        }

        var fullPath = Path.Combine(basePath, request.StorageKey.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
        {
            return null;
        }

        var fileBytes = await File.ReadAllBytesAsync(fullPath, cancellationToken);
        return new FileReadResult(fileBytes, request.ContentType, request.FileName);
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        return string.IsNullOrWhiteSpace(sanitized) ? "file" : sanitized;
    }

    private static string ComputeHash(byte[] content)
    {
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(content);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
