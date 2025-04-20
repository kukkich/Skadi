using Skadi.Matrices.Sparse;
using Skadi.Matrices.Sparse.Decompositions;

namespace Skadi.Tests.Matrices.Sparse.Decompositions;

[TestOf(typeof(IncompleteLU))]
public class IncompleteLUTest
{
    private const double Tolerance = 1e-3;
    private CSRMatrix matrix = null!;
    private CSRMatrix smallMatrix = null!;
    private readonly double[] valuesExpected = 
    [
        10, -4, 1, 1,
        -1, 8, 2,
        0.4, 19.8, -3.4,
        -2, 7.323, 6.748,
        0.5, -1, 0.5055, 8.441,
        -0.7, -6.2, 2.354, 0.497, 2.152, -2.863
    ];
    // small for easy debug
    private readonly double[] smallValuesExpected = 
    [
        10, -1, 1, 2,
        0.6, 2.6, 3.4, -7.2,
        0.1, -1.5, 8, -14,
        -0.2, 1.077, -0.183, 11.596
    ];
    
    [SetUp]
    public void Setup()
    {
        matrix = new CSRMatrix
        (
            [0, 4, 7, 10, 13, 17, 23],
            [
                0, 1, 3, 5,
                1, 2, 4,
                0, 2, 5, 
                1, 3, 5,
                0, 1, 2, 4,
                0, 1, 2, 3, 4, 5
            ],
            [
                10, -4, 1, 1,
                -1, 8, 2, 
                4, 7, -3, 
                2, 7, 4,
                5, -1, 2, 8,
                -7, 9, -3, 2, 14, -5
            ]
        );
        
        smallMatrix = new CSRMatrix(
            [0, 4, 8, 12, 16],
            [0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3],
            [10, -1, 1, 2, 6, 2, 4, -6, 1, -4, 3, -3, -2, 3, 2, 6]
        );
    }
    
    [Test]
    public void DecompositionPassWithNoErrors()
    {
        _ = IncompleteLU.Decompose(matrix);

        Assert.Pass();
    }
    
    [Test]
    public void ValuesShouldBeCorrect_Small()
    {
        var lu = IncompleteLU.Decompose(smallMatrix);
        
        Assert.Multiple(() =>
        {
            for (var i = 0; i < smallValuesExpected.Length; i++)
            {
                Assert.That(lu.Values[i], Is.EqualTo(smallValuesExpected[i]).Within(Tolerance));
            }
        });
    }
}