using Skadi.Matrices.Sparse;
using Skadi.Matrices.Sparse.Decompositions;

namespace Skadi.Tests.Matrices.Sparse.Decompositions;

public class HolleskyDecompositionTest
{
    private SymmetricRowSparseMatrix matrix = null!;
    // https://translated.turbopages.org/proxy_u/en-ru.ru.4346ee0d-67cedabb-2ffd683b-74722d776562/https/en.wikipedia.org/wiki/Incomplete_Cholesky_factorization
    private double[] DiagonalExpected = [2.24, 2.05, 2.01, 1.79, 1.33];
    private double[] ValuesExpected = [-0.89, -0.98, -0.89, -0.99, -0.89, -1.56];
    [SetUp]
    public void Setup()
    {
        matrix = new SymmetricRowSparseMatrix(
            [0, 0, 1, 2, 4, 6],
            [0, 1, 0, 2, 0, 3],
            [-2d, -2, -2, -2, -2, -2],
            [5d, 5, 5, 5, 5]
        );
    }

    [Test]
    public void DecompositionPassWithNoErrors()
    {
        _ = IncompleteHollesky.Decompose(matrix);

        Assert.Pass();
    }
    
    [Test]
    public void DiagonalShouldBeCorrect()
    {
        var l = IncompleteHollesky.Decompose(matrix);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < DiagonalExpected.Length; i++)
            {
                Assert.That(l.Diagonal[i], Is.EqualTo(DiagonalExpected[i]).Within(1e-2));
            }
        });
    }
    
    [Test]
    public void NonDiagonalValuesShouldBeCorrect()
    {
        var l = IncompleteHollesky.Decompose(matrix);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < ValuesExpected.Length; i++)
            {
                Assert.That(l.Values[i], Is.EqualTo(ValuesExpected[i]).Within(1e-2));
            }
        });
    }
}