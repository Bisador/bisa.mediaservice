namespace MediaService.Api.Controllers;

public sealed class UploadAttachmentRequest
{
    public IFormFile File { get; set; } = default!;

    public string OwnerType { get; set; } = default!;

    public Guid OwnerEntityId { get; set; }
}