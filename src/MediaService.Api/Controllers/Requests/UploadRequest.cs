using MediaService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Controllers.Requests;

public record UploadRequest
{  
    public required  IFormFile File { get; init; }
    
    public string? Url { get; init; } 
     
} 

public sealed class LinkMediaRequest
{
    public string OwnerType { get; init; } = default!;

    public Guid OwnerId { get; init; } 
}