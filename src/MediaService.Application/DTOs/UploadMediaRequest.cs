
namespace MediaService.Application.DTOs;

public sealed record UploadMediaRequest(IFormFile File, string? OwnerId);