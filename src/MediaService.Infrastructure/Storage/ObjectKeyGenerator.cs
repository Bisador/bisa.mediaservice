namespace MediaService.Infrastructure.Storage;

public static class ObjectKeyGenerator
{
    public static string Generate(Guid tenantId, string originalFileName)
    {
        var ext = Path.GetExtension(originalFileName);

        var guid = Guid.NewGuid().ToString("N");

        return
            $"{tenantId}/{DateTime.UtcNow:yyyy/MM/dd}/{guid}{ext}";
    }
}