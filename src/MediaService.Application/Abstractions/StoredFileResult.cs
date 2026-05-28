namespace MediaService.Application.Abstractions;

public sealed record StoredFileResult(
    string BucketName,
    string ObjectKey,
    string PublicUrl,
    string? ETag = null);
     