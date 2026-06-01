namespace MediaService.Application.Commands;

public sealed record ValidateMediaCommand(
    Guid TenantId,
    IReadOnlyCollection<Guid> MediaIds);