using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.Tests.LinAlTests;

public class MatrixOpsTests
{
    [Test]
    public void ScaleShouldMultiplyEachElementByCoefficient()
    {
        var matrix = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });

        var result = MatrixOps.Scale(2d, matrix);

        Assert.That(result[0, 0], Is.EqualTo(2d));
        Assert.That(result[0, 1], Is.EqualTo(4d));
        Assert.That(result[1, 0], Is.EqualTo(6d));
        Assert.That(result[1, 1], Is.EqualTo(8d));
    }

    [Test]
    public void SumShouldAddElementwise()
    {
        var a = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        var b = new Matrix(new double[,] { { 5, 6 }, { 7, 8 } });

        var result = MatrixOps.Sum(a, b);

        Assert.That(result[0, 0], Is.EqualTo(6d));
        Assert.That(result[1, 1], Is.EqualTo(12d));
    }

    [Test]
    public void MultiplyShouldSupportNonSquareMatrices()
    {
        // 2x3 * 3x2 = 2x2
        var a = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });
        var b = new Matrix(new double[,] { { 7, 8 }, { 9, 10 }, { 11, 12 } });

        var result = MatrixOps.Multiply(a, b);

        Assert.That(result.Rows, Is.EqualTo(2));
        Assert.That(result.Columns, Is.EqualTo(2));
        Assert.That(result[0, 0], Is.EqualTo(1 * 7 + 2 * 9 + 3 * 11));
        Assert.That(result[0, 1], Is.EqualTo(1 * 8 + 2 * 10 + 3 * 12));
        Assert.That(result[1, 0], Is.EqualTo(4 * 7 + 5 * 9 + 6 * 11));
        Assert.That(result[1, 1], Is.EqualTo(4 * 8 + 5 * 10 + 6 * 12));
    }

    [Test]
    public void MultiplyShouldRejectMismatchedInnerDimension()
    {
        var a = new Matrix(new double[,] { { 1, 2 } });
        var b = new Matrix(new double[,] { { 1, 2 } });

        Assert.Throws<ArgumentException>(() => MatrixOps.Multiply(a, b));
    }

    [Test]
    public void MultiplyOnShouldSupportNonSquareMatrices()
    {
        // 2x3 matrix times a 3-vector must use Rows for the outer loop and Columns for the inner one.
        var a = new Matrix(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });

        var result = MatrixOps.MultiplyOn(a, new Skadi.LinearAlgebra.Vectors.Vector(1, 1, 1));

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.ToArray(), Is.EqualTo(new[] { 6d, 15d }).AsCollection);
    }

    [Test]
    public void SpanKernelsShouldMatchHeapConvenienceOverloads()
    {
        var heap = new Matrix(new double[,] { { 1, 2 }, { 3, 4 } });
        double[] spanValues = [1, 2, 3, 4];
        var span = new ReadOnlyMatrixSpan(spanValues, 2);

        Span<double> scaled = stackalloc double[4];
        MatrixOps.Scale(2d, span, new MatrixSpan(scaled, 2));

        var heapScaled = MatrixOps.Scale(2d, heap);
        Assert.That(scaled.ToArray(), Is.EqualTo(new[] { heapScaled[0, 0], heapScaled[0, 1], heapScaled[1, 0], heapScaled[1, 1] }).AsCollection);
    }

    [Test]
    public void MultiplySpanKernelShouldComputeMatrixVectorProduct()
    {
        double[] values = [1, 2, 3, 4];
        var matrix = new ReadOnlyMatrixSpan(values, 2);
        ReadOnlySpan<double> vector = [1, 1];
        Span<double> destination = stackalloc double[2];

        MatrixOps.Multiply(matrix, vector, destination);

        Assert.That(destination.ToArray(), Is.EqualTo(new[] { 3d, 7d }).AsCollection);
    }
}
