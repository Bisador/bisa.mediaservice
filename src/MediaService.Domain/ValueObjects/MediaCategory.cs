namespace MediaService.Domain.ValueObjects;

public sealed record MediaCategory
{
    public string Value { get; }

    public MediaCategory(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidCategoryException();

        Value = value.Trim().ToLowerInvariant();
    }

    public override string ToString()
        => Value;
}

public class InvalidCategoryException() : Exception("Invalid Category");