using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.Matrices.Sparse;

public class SymmetricRowSparseMatrixMultiplyOnTest
{
    [Test]
    public void MultiplyOnShouldMatchTheDenseEquivalent()
    {
        // Dense equivalent:
        // {1, 0, 7, 0, 8, 0},
        // {0, 2, 9, 0, 0, 10},
        // {7, 9, 3, 11, 12, 13},
        // {0, 0, 11, 4, 0, 0},
        // {8, 0, 12, 0, 5, 14},
        // {0, 10, 13, 0, 14, 6},
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle
        (
            upperRowPointers: [0, 2, 4, 7, 7, 8, 8],
            upperColumnIndexes: [2, 4, 2, 5, 3, 4, 5, 5],
            upperValues: [7, 8, 9, 10, 11, 12, 13, 14],
            diagonal: [1, 2, 3, 4, 5, 6]
        );

        var result = matrix.MultiplyOn(new Vector(1, 2, 3, 4, 5, 6));

        Assert.That(result.ToArray(), Is.EqualTo(new[] { 62d, 91d, 216d, 49d, 153d, 165d }).AsCollection);
    }
}
