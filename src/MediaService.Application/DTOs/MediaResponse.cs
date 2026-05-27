namespace MediaService.Application.DTOs;

public sealed record MediaResponse(
    Guid Id,
    string OriginalFileName,
    string ContentType,
    long Size,
    string Url,
    DateTimeOffset CreatedAt);
    
    