namespace DevcraftWMS.Application.Features.Asns;

public sealed record AsnAttachmentDto(
    Guid Id,
    Guid AsnId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc);
