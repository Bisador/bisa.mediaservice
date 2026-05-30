using MediaService.Domain.Models;

namespace MediaService.Application.Commands;

public sealed record UploadAttachmentCommand(
    IFormFile File,
    OwnerReference Owner,
    string? OwnerId);