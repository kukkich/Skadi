using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.LinAlTests;

public class VectorOpsTests
{
    // 10 elements: exercises both the SIMD-width loop and the scalar tail.
    private static readonly Vector A = new(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    private static readonly Vector B = new(10, 9, 8, 7, 6, 5, 4, 3, 2, 1);

    [Test]
    public void SumShouldAddElementwise()
    {
        var result = VectorOps.Sum(A, B);

        Assert.That(result.ToArray(), Is.All.EqualTo(11d));
    }

    [Test]
    public void SubtractShouldSubtractElementwise()
    {
        var result = VectorOps.Subtract(A, B);

        double[] expected = [-9, -7, -5, -3, -1, 1, 3, 5, 7, 9];
        Assert.That(result.ToArray(), Is.EqualTo(expected).AsCollection);
    }

    [Test]
    public void LinearCombinationShouldReuseSuppliedDestination()
    {
        var destination = Vector.Create(A.Count);

        var result = VectorOps.LinearCombination(A, B, 2d, 3d, destination);

        Assert.That(result, Is.SameAs(destination));
        Assert.That(result.ToArray(), Is.EqualTo(new[] { 32d, 31, 30, 29, 28, 27, 26, 25, 24, 23 }).AsCollection);
    }

    [Test]
    public void ScaleShouldMultiplyEachElementByCoefficient()
    {
        var result = VectorOps.Scale(2d, A);

        Assert.That(result.ToArray(), Is.EqualTo(new[] { 2d, 4, 6, 8, 10, 12, 14, 16, 18, 20 }).AsCollection);
    }

    [Test]
    public void KernelShouldMatchConvenienceOverload()
    {
        Span<double> destination = stackalloc double[A.Count];

        VectorOps.LinearCombination(A, B, 1d, -1d, destination);

        Assert.That(destination.ToArray(), Is.EqualTo(VectorOps.Subtract(A, B).ToArray()).AsCollection);
    }

    [Test]
    public void SumShouldThrowOnLengthMismatch()
    {
        var shorter = new Vector(1, 2, 3);

        Assert.Throws<ArgumentException>(() => VectorOps.Sum(A, shorter));
    }

    [Test]
    public void MultiplyAsTransparentShouldReturnDotProduct()
    {
        ReadOnlySpan<double> v = [1, 2, 3];
        ReadOnlySpan<double> u = [4, 5, 6];

        var result = VectorOps.MultiplyAsTransparent<double, double, double>(v, u);

        Assert.That(result, Is.EqualTo(1 * 4 + 2 * 5 + 3 * 6));
    }
}
