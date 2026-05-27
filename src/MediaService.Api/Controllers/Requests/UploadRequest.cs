using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Controllers.Requests;

public record UploadRequest
{  
    public required  IFormFile File { get; init; }
    
    public string? Url { get; init; } 
} 