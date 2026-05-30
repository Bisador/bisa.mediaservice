using MediaService.Domain.Models;

namespace MediaService.Application.Commands;

public sealed record LinkMediaCommand(
    Guid MediaId,
    OwnerReference Owner);