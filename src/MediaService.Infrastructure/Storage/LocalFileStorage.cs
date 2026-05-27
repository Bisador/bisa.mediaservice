using MediaService.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace MediaService.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _basePath;
    private readonly string _publicBaseUrl;

    public LocalFileStorage(IConfiguration configuration)
    {
        _basePath = configuration["Storage:BasePath"] ?? Path.Combine(AppContext.BaseDirectory, "uploads");
        _publicBaseUrl = configuration["Storage:PublicBaseUrl"] ?? "http://localhost:5000/media-files";

        Directory.CreateDirectory(_basePath);
    }

    public async Task<StoredFileResult> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(originalFileName);
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(_basePath, storedFileName);

        await using var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fs, cancellationToken);

        var publicUrl = $"{_publicBaseUrl.TrimEnd('/')}/{storedFileName}";
        return new StoredFileResult(storedFileName, fullPath, publicUrl);
    }

    public Task<Stream> OpenReadAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        Stream stream = File.OpenRead(storagePath);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(storagePath))
            File.Delete(storagePath);

        return Task.CompletedTask;
    }
}