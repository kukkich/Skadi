using NUnit.Framework.Legacy;
using Skadi.Matrices;
using Skadi.Matrices.Sparse;

// ReSharper disable InconsistentNaming

namespace Skadi.Tests.Matrices.Sparse;

public class SymmetricRowSparseMatrixTest
{
    private Matrix originalMatrix = null!;
    private double[] diagonal = null!;
    private double[] upperValues = null!;
    private int[] upperColumnIndexes = null!;
    private int[] upperRowPointers = null!;
    
    [SetUp]
    public void Setup()
    {
        originalMatrix = new Matrix(new double[,]
        {
            {1, 0, 7, 0, 8, 0},
            {0, 2, 9, 0, 0, 10},
            {7, 9, 3, 11, 12, 13},
            {0, 0, 11, 4, 0, 0},
            {8, 0, 12, 0, 5, 14},
            {0, 10, 13, 0, 14, 6},
        });
        diagonal = [1, 2, 3, 4, 5, 6];
        upperValues = [7, 8, 9, 10, 11, 12, 13, 14];
        upperColumnIndexes = [2, 4, 2, 5, 3, 4, 5, 5];
        upperRowPointers = [0, 2, 4, 7, 7, 8, 8];
    }

    [Test]
    public void ValuesShouldBeCorrect()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes, upperValues, diagonal);
        double[] expectedValues = [7, 9, 11, 8, 12, 10, 13, 14];

        Assert.That(matrix.Values, Is.EqualTo(expectedValues).AsCollection);
    }
    
    [Test]
    public void DiagonalShouldBeTheSame()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes, upperValues, diagonal);
        double[] expectedDiagonal = [1, 2, 3, 4, 5, 6];

        Assert.That(matrix.Diagonal, Is.EqualTo(expectedDiagonal).AsCollection);
    }
    
    [Test]
    public void ColumnIndexesShouldBeCorrect()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes, upperValues, diagonal);
        int[] expectedColumnIndexes = [0, 1, 2, 0, 2, 1, 2, 4];

        Assert.That(matrix.ColumnIndexes.ToArray(), Is.EqualTo(expectedColumnIndexes).AsCollection);
    }
    
    [Test]
    public void RowPointersShouldBeCorrect()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes, upperValues, diagonal);
        int[] expectedRowPointers = [0, 0, 0, 2, 3, 5, 8];

        Assert.That(matrix.RowPointers.ToArray(), Is.EqualTo(expectedRowPointers).AsCollection);
    }
    
    [Test]
    public void AllValuesShouldBeTheSame()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes, upperValues, diagonal);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < originalMatrix.Rows; i++)
            for (var j = 0; j < originalMatrix.Columns; j++)
            {
                Assert.That(matrix.GetValue(i, j), Is.EqualTo(originalMatrix[i, j]));
            }
        });
    }
    
    [Test]
    public void RowPointersShouldBeCorrect_PortraitOnly()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes);
        int[] expectedRowPointers = [0, 0, 0, 2, 3, 5, 8];

        Assert.That(matrix.RowPointers.ToArray(), Is.EqualTo(expectedRowPointers).AsCollection);
    }
    
    [Test]
    public void ColumnIndexesShouldBeCorrect_PortraitOnly()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes);
        int[] expectedColumnIndexes = [0, 1, 2, 0, 2, 1, 2, 4];

        Assert.That(matrix.ColumnIndexes.ToArray(), Is.EqualTo(expectedColumnIndexes).AsCollection);
    }
    
    [Test]
    public void AllValuesShouldBeZeroWhenPortraitOnly()
    {
        var matrix = SymmetricRowSparseMatrix.FromUpperTriangle(upperRowPointers, upperColumnIndexes);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < originalMatrix.Rows; i++)
            for (var j = 0; j < originalMatrix.Columns; j++)
            {
                Assert.That(matrix.GetValue(i, j), Is.EqualTo(0));
            }
        });
    }
}