namespace MediaService.Application.Commands;

public sealed record MetadataMediaCommand(Guid TenantId, Guid Id);
public sealed record MetadataBatchMediaCommand(Guid TenantId, List<Guid> Ids);
