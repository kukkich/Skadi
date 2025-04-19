using Skadi.Matrices;
using Skadi.Vectors;

namespace Skadi.Tests.LinAlTests.MVOperations;

[TestOf(typeof(LinAl))]
public class CSRMastrixOperationsTests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Test1()
    {
        var matrix = new CSRMatrix
        (
            [0, 3, 6, 9, 11, 14, 20],
            [0, 1, 3, 1, 2, 4, 0, 2, 5, 1, 3, 0, 1, 2, 0, 1, 2, 3, 4, 5],
            [1, -4, 1, -1, 8, 2, 4, 3, -3, 2, 7, 5, -1, 2, -7, 9, -3, 2, 14, -5]
        );
        var vector = new Vector(1, 2, 3, 4, 5, 6);
        var expected = new Vector(-3, 32, -5, 32, 9, 50);
        
        var actual = LinAl.Multiply(matrix, vector);
        
        Assert.That(actual.ToArray(), Is.EqualTo(expected.ToArray()).AsCollection);
        Assert.Pass();
    }
}