using Skadi.LinearAlgebra;

namespace Skadi.Tests.LinAlTests;

public class ShapeTests
{
    [Test]
    public void SameLengthShouldNotThrowWhenEqual()
    {
        Assert.DoesNotThrow(() => Shape.SameLength(3, 3));
    }

    [Test]
    public void SameLengthShouldThrowWhenDifferent()
    {
        Assert.Throws<ArgumentException>(() => Shape.SameLength(3, 4));
    }

    [Test]
    public void SameSizeShouldNotThrowWhenEqual()
    {
        Assert.DoesNotThrow(() => Shape.SameSize(2, 3, 2, 3));
    }

    [TestCase(3, 3, 2, 3)]
    [TestCase(3, 3, 3, 2)]
    public void SameSizeShouldThrowWhenDifferent(int aRows, int aColumns, int bRows, int bColumns)
    {
        Assert.Throws<ArgumentException>(() => Shape.SameSize(aRows, aColumns, bRows, bColumns));
    }

    [Test]
    public void ConformantShouldNotThrowWhenMultipliable()
    {
        Assert.DoesNotThrow(() => Shape.Conformant(3, 3));
    }

    [Test]
    public void ConformantShouldThrowWhenNotMultipliable()
    {
        Assert.Throws<ArgumentException>(() => Shape.Conformant(3, 4));
    }

    [Test]
    public void NotSameInstanceShouldThrowForTheSameReference()
    {
        var instance = new object();

        Assert.Throws<ArgumentException>(() => Shape.NotSameInstance(instance, instance));
    }

    [Test]
    public void NotSameInstanceShouldNotThrowForDifferentReferences()
    {
        Assert.DoesNotThrow(() => Shape.NotSameInstance(new object(), new object()));
    }
}
