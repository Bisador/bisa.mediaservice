namespace MediaService.Application.DTOs;

public sealed record MediaValidationResponse(
    IReadOnlyCollection<Guid> ValidMediaIds,
    IReadOnlyCollection<Guid> InvalidMediaIds);
    
// public sealed record MediaValidationResponse(
//     Guid MediaId,
//     bool Exists,
//     bool IsDeleted,
//     bool IsAvailable,
//     MediaCategory Category,
//     Guid TenantId);