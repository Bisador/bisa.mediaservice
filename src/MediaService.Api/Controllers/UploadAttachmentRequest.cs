namespace MediaService.Api.Controllers;

public sealed record UploadAttachmentRequest(
    IFormFile File,
    string OwnerType,
    string OwnerId);