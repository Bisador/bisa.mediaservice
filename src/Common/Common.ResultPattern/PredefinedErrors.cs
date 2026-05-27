namespace Common.ResultPattern;

public static class PredefinedErrors
{
    public static Error NotFound(string? message = null) => new ItemNotFoundError(message);
    public static Error Duplicate(string? message = null) => new DuplicateItemError(message);
}

public record ItemNotFoundError(string? Message = null) : Error(nameof(ItemNotFoundError), Message ?? "مورد یافت نشد");
public record DuplicateItemError(string? Message = null) : Error(nameof(DuplicateItemError), Message ?? "مورد مشابه وجود دارد");