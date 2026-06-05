namespace MediaService.Application.Commands;

public sealed record RemoveMediaLinkByOwnerCommand(
    Guid TenantId,
    Guid MediaId,
    string OwnerType,
    string OwnerId);