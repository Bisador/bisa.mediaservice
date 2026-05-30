namespace MediaService.Application.DTOs;

public sealed record MediaValidationResponse(
    IReadOnlyCollection<Guid> ValidMediaIds,
    IReadOnlyCollection<Guid> InvalidMediaIds);
    
    