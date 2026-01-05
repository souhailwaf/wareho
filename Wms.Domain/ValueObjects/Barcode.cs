// Wms.Domain/ValueObjects/Barcode.cs

using Wms.Domain.Common;

namespace Wms.Domain.ValueObjects;

public class Barcode : ValueObject
{
    public Barcode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Barcode cannot be empty", nameof(value));

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < 3)
            throw new ArgumentException("Barcode must be at least 3 characters", nameof(value));

        if (trimmedValue.Length > 50)
            throw new ArgumentException("Barcode cannot exceed 50 characters", nameof(value));

        Value = trimmedValue.ToUpperInvariant();
    }

    public string Value { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Barcode barcode)
    {
        return barcode.Value;
    }

    public static implicit operator Barcode(string value)
    {
        return new Barcode(value);
    }
}