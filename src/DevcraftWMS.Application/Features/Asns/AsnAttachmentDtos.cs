namespace DevcraftWMS.Application.Features.Asns;

public sealed record AsnAttachmentDto(
    Guid Id,
    Guid AsnId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string? StorageUrl,
    DateTime CreatedAtUtc);

public sealed record AsnAttachmentDownloadDto(
    Guid Id,
    string FileName,
    string ContentType,
    byte[] Content);
