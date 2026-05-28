namespace MediaService.Infrastructure.Storage.LocalStorage;

public sealed class LocalStorageProviderOptions
{
    public string BasePath { get; set; } = Path.Combine(AppContext.BaseDirectory, "uploads"); 
}