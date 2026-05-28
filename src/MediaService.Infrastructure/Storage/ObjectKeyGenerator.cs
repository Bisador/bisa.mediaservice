namespace MediaService.Infrastructure.Storage;

public static class ObjectKeyGenerator
{
    public static string Generate(string originalFileName)
    {
        var ext = Path.GetExtension(originalFileName);

        var guid = Guid.NewGuid().ToString("N");

        return
            $"{DateTime.UtcNow:yyyy/MM/dd}/{guid}{ext}";
    }
}