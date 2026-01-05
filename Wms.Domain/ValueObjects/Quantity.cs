// Wms.Domain/ValueObjects/Quantity.cs

using Wms.Domain.Common;

namespace Wms.Domain.ValueObjects;

public class Quantity : ValueObject, IComparable<Quantity>
{
    public static readonly Quantity Zero = new(0);

    public Quantity(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        Value = Math.Round(value, 4);
    }

    public decimal Value { get; }

    public int CompareTo(Quantity? other)
    {
        if (other is null) return 1;
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString("0.##");
    }

    public static implicit operator decimal(Quantity quantity)
    {
        return quantity.Value;
    }

    public static implicit operator Quantity(decimal value)
    {
        return new Quantity(value);
    }

    public static Quantity operator +(Quantity left, Quantity right)
    {
        return new Quantity(left.Value + right.Value);
    }

    public static Quantity operator -(Quantity left, Quantity right)
    {
        var result = left.Value - right.Value;
        if (result < 0)
            throw new ArgumentException("Operation would result in negative quantity");
        return new Quantity(result);
    }

    public static Quantity operator *(Quantity left, decimal right)
    {
        if (right < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(right));
        return new Quantity(left.Value * right);
    }

    public static bool operator >(Quantity left, Quantity right)
    {
        return left.Value > right.Value;
    }

    public static bool operator <(Quantity left, Quantity right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >=(Quantity left, Quantity right)
    {
        return left.Value >= right.Value;
    }

    public static bool operator <=(Quantity left, Quantity right)
    {
        return left.Value <= right.Value;
    }
}