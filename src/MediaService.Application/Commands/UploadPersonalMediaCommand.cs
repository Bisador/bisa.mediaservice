namespace MediaService.Application.Commands;

public sealed record UploadPersonalMediaCommand(
    IFormFile File,
    string? OwnerId);