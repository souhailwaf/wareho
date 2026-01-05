// Wms.Domain.Tests/ValueObjects/QuantityTests.cs

using Wms.Domain.ValueObjects;

namespace Wms.Domain.Tests.ValueObjects;

public class QuantityTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10.5)]
    [InlineData(999.99)]
    public void Constructor_WithValidValue_CreatesQuantitySuccessfully(decimal validValue)
    {
        // Act
        var quantity = new Quantity(validValue);

        // Assert
        quantity.Value.Should().Be(validValue);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNegativeValue_ThrowsArgumentException(decimal negativeValue)
    {
        // Act & Assert
        var act = () => new Quantity(negativeValue);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Add_WithAnotherQuantity_ReturnsCorrectSum()
    {
        // Arrange
        var quantity1 = new Quantity(10.5m);
        var quantity2 = new Quantity(5.25m);

        // Act
        var result = quantity1 + quantity2;

        // Assert
        result.Value.Should().Be(15.75m);
    }

    [Fact]
    public void Subtract_WithSmallerQuantity_ReturnsCorrectDifference()
    {
        // Arrange
        var quantity1 = new Quantity(10.5m);
        var quantity2 = new Quantity(3.25m);

        // Act
        var result = quantity1 - quantity2;

        // Assert
        result.Value.Should().Be(7.25m);
    }

    [Fact]
    public void Subtract_WithLargerQuantity_ThrowsArgumentException()
    {
        // Arrange
        var quantity1 = new Quantity(5.0m);
        var quantity2 = new Quantity(10.0m);

        // Act & Assert
        var act = () => quantity1 - quantity2;
        act.Should().Throw<ArgumentException>()
            .WithMessage("*result in negative quantity*");
    }

    [Fact]
    public void Multiply_WithPositiveMultiplier_ReturnsCorrectProduct()
    {
        // Arrange
        var quantity = new Quantity(10.0m);
        var multiplier = 2.5m;

        // Act
        var result = quantity * multiplier;

        // Assert
        result.Value.Should().Be(25.0m);
    }

    [Fact]
    public void Multiply_WithZeroMultiplier_ReturnsZeroQuantity()
    {
        // Arrange
        var quantity = new Quantity(10.0m);
        var multiplier = 0m;

        // Act
        var result = quantity * multiplier;

        // Assert
        result.Value.Should().Be(0m);
    }

    [Fact]
    public void Multiply_WithNegativeMultiplier_ThrowsArgumentException()
    {
        // Arrange
        var quantity = new Quantity(10.0m);
        var multiplier = -2.0m;

        // Act & Assert
        var act = () => quantity * multiplier;
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Equals_WithSameValue_ReturnsTrue()
    {
        // Arrange
        var quantity1 = new Quantity(10.5m);
        var quantity2 = new Quantity(10.5m);

        // Act & Assert
        quantity1.Should().Be(quantity2);
        (quantity1 == quantity2).Should().BeTrue();
        (quantity1 != quantity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentValue_ReturnsFalse()
    {
        // Arrange
        var quantity1 = new Quantity(10.5m);
        var quantity2 = new Quantity(5.25m);

        // Act & Assert
        quantity1.Should().NotBe(quantity2);
        (quantity1 == quantity2).Should().BeFalse();
        (quantity1 != quantity2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_WithSmallerQuantity_ReturnsPositive()
    {
        // Arrange
        var quantity1 = new Quantity(10.0m);
        var quantity2 = new Quantity(5.0m);

        // Act
        var result = quantity1.CompareTo(quantity2);

        // Assert
        result.Should().BePositive();
        (quantity1 > quantity2).Should().BeTrue();
        (quantity1 >= quantity2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_WithLargerQuantity_ReturnsNegative()
    {
        // Arrange
        var quantity1 = new Quantity(5.0m);
        var quantity2 = new Quantity(10.0m);

        // Act
        var result = quantity1.CompareTo(quantity2);

        // Assert
        result.Should().BeNegative();
        (quantity1 < quantity2).Should().BeTrue();
        (quantity1 <= quantity2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_WithEqualQuantity_ReturnsZero()
    {
        // Arrange
        var quantity1 = new Quantity(10.0m);
        var quantity2 = new Quantity(10.0m);

        // Act
        var result = quantity1.CompareTo(quantity2);

        // Assert
        result.Should().Be(0);
        (quantity1 >= quantity2).Should().BeTrue();
        (quantity1 <= quantity2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ReturnsFormattedValue()
    {
        // Arrange
        var quantity = new Quantity(10.5m);

        // Act
        var result = quantity.ToString();

        // Assert
        result.Should().Be("10.5");
    }

    [Fact]
    public void ImplicitConversion_FromDecimal_CreatesQuantitySuccessfully()
    {
        // Arrange
        var value = 10.5m;

        // Act
        Quantity quantity = value;

        // Assert
        quantity.Value.Should().Be(value);
    }

    [Fact]
    public void ImplicitConversion_ToDecimal_ReturnsValue()
    {
        // Arrange
        var quantity = new Quantity(10.5m);

        // Act
        decimal value = quantity;

        // Assert
        value.Should().Be(10.5m);
    }
}