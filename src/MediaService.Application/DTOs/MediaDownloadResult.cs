namespace MediaService.Application.DTOs;

public record MediaDownloadResult(Stream Stream, string ContentType, string FileName);