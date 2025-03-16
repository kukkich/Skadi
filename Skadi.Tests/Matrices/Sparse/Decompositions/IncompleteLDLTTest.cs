using Skadi.Matrices.Sparse;
using Skadi.Matrices.Sparse.Decompositions;
// ReSharper disable InconsistentNaming

namespace Skadi.Tests.Matrices.Sparse.Decompositions;

public class IncompleteLDLTTest
{
    private const double Tolerance = 1e-10;
    private SymmetricRowSparseMatrix matrix = null!;
    private SymmetricRowSparseMatrix smallMatrix = null!;
    private readonly double[] diagonalExpected = [3, -5, 11, -9, 4];
    private readonly double[] valuesExpected = [4, -4, 8, 2, 2, -6, 1, -4, 8, 5];
    // small for easy debug
    private readonly double[] smallDiagonalExpected = [4, 1, 9];
    private readonly double[] smallValuesExpected = [3, -4, 5];
    
    [SetUp]
    public void Setup()
    {
        matrix = new SymmetricRowSparseMatrix(
            [0, 0, 1, 3, 6, 10],
            [0, 0, 1, 0, 1, 2, 0, 1, 2, 3],
            [12, -12, -88, 6, 14, -170, 3, 32, 236, -527],
            [3, 43, -261, 379, 406]
        );
        
        smallMatrix = new SymmetricRowSparseMatrix(
            [0, 0, 1, 3],
            [0, 0, 1],
            [12, -16, -43],
            [4, 37, 98]
        );
    }

    [Test]
    public void DecompositionPassWithNoErrors()
    {
        _ = IncompleteLDLT.Decompose(matrix);

        Assert.Pass();
    }
    
    [Test]
    public void DiagonalShouldBeCorrect()
    {
        var l = IncompleteLDLT.Decompose(matrix);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < diagonalExpected.Length; i++)
            {
                Assert.That(l.Diagonal[i], Is.EqualTo(diagonalExpected[i]).Within(Tolerance));
            }
        });
    }
    
    [Test]
    public void NonDiagonalValuesShouldBeCorrect()
    {
        var l = IncompleteLDLT.Decompose(matrix);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < valuesExpected.Length; i++)
            {
                Assert.That(l.Values[i], Is.EqualTo(valuesExpected[i]).Within(Tolerance));
            }
        });
    }
    
    [Test]
    public void DiagonalShouldBeCorrect_Small()
    {
        var l = IncompleteLDLT.Decompose(smallMatrix);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < smallDiagonalExpected.Length; i++)
            {
                Assert.That(l.Diagonal[i], Is.EqualTo(smallDiagonalExpected[i]).Within(Tolerance));
            }
        });
    }
    
    [Test]
    public void NonDiagonalValuesShouldBeCorrect_Small()
    {
        var l = IncompleteLDLT.Decompose(smallMatrix);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < smallValuesExpected.Length; i++)
            {
                Assert.That(l.Values[i], Is.EqualTo(smallValuesExpected[i]).Within(Tolerance));
            }
        });
    }
}