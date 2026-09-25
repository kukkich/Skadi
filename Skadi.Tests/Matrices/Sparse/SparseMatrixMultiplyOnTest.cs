using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.Matrices.Sparse;

public class SparseMatrixMultiplyOnTest
{
    [Test]
    public void MultiplyOnShouldMatchTheDenseEquivalent()
    {
        // Dense equivalent: [[2, 7], [5, 3]]
        var matrix = new SparseMatrix
        (
            rowsIndexes: [0, 0, 1],
            columnsIndexes: [0],
            diagonal: [2, 3],
            lowerValues: [5],
            upperValues: [7]
        );

        var result = matrix.MultiplyOn(new Vector(1, 2));

        Assert.That(result.ToArray(), Is.EqualTo(new[] { 16d, 11d }).AsCollection);
    }

    [Test]
    public void MultiplyOnShouldNotAccumulateIntoAReusedDestination()
    {
        var matrix = new SparseMatrix(rowsIndexes: [0, 0, 1], columnsIndexes: [0], diagonal: [2, 3], lowerValues: [5], upperValues: [7]);
        var destination = new Vector(100, 100);

        var result = matrix.MultiplyOn(new Vector(1, 2), destination);

        Assert.That(result.ToArray(), Is.EqualTo(new[] { 16d, 11d }).AsCollection);
    }
}
