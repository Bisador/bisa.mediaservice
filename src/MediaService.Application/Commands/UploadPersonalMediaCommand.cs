namespace MediaService.Application.Commands;

public sealed record UploadPersonalMediaCommand(
    Guid TenantId, 
    IFormFile File,
    string? OwnerId);