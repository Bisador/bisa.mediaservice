namespace MediaService.Application.Commands;

public sealed record MetadataMediaCommand(Guid TenantId, Guid Id);