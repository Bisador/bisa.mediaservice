using MediaService.Domain.Enums;

namespace MediaService.Application.Commands;

public sealed record UploadPersonalMediaCommand(
    Guid TenantId, 
    IFormFile File,
    MediaAccessLevel? AccessLevel,
    string? OwnerId);