namespace MediaService.Application.Services;

public static class ObjectKeyGenerator
{
    public static string Generate(Guid tenantId, string category, string originalFileName)
    {
        var ext = Path.GetExtension(originalFileName);
 
        return
            $"{tenantId}/" +
            $"{category.ToLowerInvariant()}/" +
            $"{DateTime.UtcNow:yyyy/MM/dd}/" +
            $"{Guid.NewGuid():N}{ext}"; 
    }
}