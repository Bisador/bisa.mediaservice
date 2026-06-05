namespace MediaService.Api.Controllers.Requests;

public sealed class RemoveLinkRequest
{
    public required string OwnerType { get; init; }
    public required string OwnerId { get; init; }
}