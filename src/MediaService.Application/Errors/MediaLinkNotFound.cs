namespace MediaService.Application.Errors;

public sealed record MediaLinkNotFound() : Error("Media.Link.NotFound", "The requested media link was not found.");