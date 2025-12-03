using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Tests.LinAlTests.MVOperations;

public class ComplexMatrixOperationsTests
{
    [Test]
    public void DefaultMultiplyOnVector1ShouldBeCorrect()
    {
        double[] di =  [1, 2, 3, 4, 5];
        double[] gg = [1, 7, 8];
        int[] idi = [0, 2, 4, 5];
        int[] ig = [0, 0, 0, 2];
        int[] jg = [0, 1];
        int[] ijg = [0, 1, 3];
        
        var matrix = new ComplexMatrix(di, gg, idi, ijg, ig, jg);
        var complexVector = new Vector(1, 1, 1, 1, 1, 1);
        
        var actual = matrix.MultiplyOn(complexVector).ToArray();
        
        var expected = new Vector(0, 4, -2, 22, 5, 21).ToArray();
        
        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
    
    [Test]
    public void DiagonalMatrixMultiplyOnVector1ShouldBeCorrect()
    {
        double[] di =  [1, 2, 3, 4, 5];
        double[] gg = [];
        int[] idi = [0, 2, 4, 5];
        int[] ig = [0, 0, 0, 0];
        int[] jg = [];
        int[] ijg = [];
        
        var matrix = new ComplexMatrix(di, gg, idi, ijg, ig, jg);
        var vector = new Vector(1, 1, 1, 1, 1, 1);
        
        var actual = matrix.MultiplyOn(vector).ToArray();
        
        var expected = new Vector(-1, 3, -1, 7, 5, 5).ToArray();
        
        Assert.That(actual, Is.EqualTo(expected).AsCollection);
    }
}