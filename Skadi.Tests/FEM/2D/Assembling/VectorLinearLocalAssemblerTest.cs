using Microsoft.Extensions.Logging.Abstractions;
using Skadi.Algorithms.Integration;
using Skadi.FEM._2D.Assembling;
using Skadi.FEM._2D.BasisFunctions;
using Skadi.FEM.Assembling;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Materials.LambdaGamma;
using Skadi.FEM.Materials.Providers;
using Skadi.FEM.Providers.Density;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.Tests.FEM._2D.Assembling;

[TestOf(typeof(VectorLinearLocalAssembler))]
public class VectorLinearLocalAssemblerTest
{
    [SetUp]
    public void Setup()
    {
        var material = new Material(1, 0);
        CreateAssembler(material);
    }

    private VectorLinearLocalAssembler CreateAssembler(Material material)
    {
        var points = new IrregularPointsCollection<Vector2D>([
            new Vector2D(1, 1),
            new Vector2D(5, 1),
            new Vector2D(1, 3),
            new Vector2D(5, 3)
        ]);
        return new VectorLinearLocalAssembler
        (
            points,
            new AreaProvider<AreaDefinition>([new AreaDefinition(0, 1, 0, 1)]),
            new Gauss2D(GaussConfig.Gauss3(1), NullLogger.Instance),
            new FromArrayMaterialProvider<Material>([material]),
            new RectangleVectorBasicFunctionsProvider(points),
            new FuncDensity<Vector2D, Vector2D>(points, x => new Vector2D(1, -1))
        );
    }

    [Test]
    public void StiffnessShouldBeCorrect()
    {
        var assembler = CreateAssembler(new Material(1, 0));
        var element = new EdgeElement(0, [0, 1, 2, 3], [0, 1, 2, 3]);
        var (hx, hy) = (4d, 2d);
        var expectedMatrix = new Matrix(new[,]
        {
            {hy/hx, -hy/hx, -1, 1},
            {-hy/hx, hy/hx, 1, -1},
            {-1, 1, hx/hy, -hx/hy},
            {1, -1, -hx/hy, hx/hy},
        });
        
        Span<double> matrixValues = stackalloc double[4 * 4];
        Span<int> permutations = [0, 0, 0, 0];
        var matrix = new MatrixSpan(matrixValues, 4);
        var permutation = new StackIndexPermutation(permutations);
        
        assembler.AssembleMatrix(element, matrix, permutation);

        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                var diff = Math.Abs(expectedMatrix[i, j] - matrix[i, j]);
                Assert.That(diff, Is.LessThanOrEqualTo(1e-4));
            }
        }
    }
    
    [Test]
    public void MassShouldBeCorrect()
    {
        var assembler = CreateAssembler(new Material(0, 1));
        var element = new EdgeElement(0, [0, 1, 2, 3], [0, 1, 2, 3]);
        var (hx, hy) = (4d, 2d);
        var expectedMatrix = LinAl.Multiply(hx * hy / 6, new Matrix(new[,]
        {
            {2d, 1, 0, 0},
            {1, 2, 0, 0},
            {0, 0, 2, 1},
            {0, 0, 1, 2}
        }));
        
        Span<double> matrixValues = stackalloc double[4 * 4];
        Span<int> permutations = [0, 0, 0, 0];
        var matrix = new MatrixSpan(matrixValues, 4);
        var permutation = new StackIndexPermutation(permutations);
        
        assembler.AssembleMatrix(element, matrix, permutation);

        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                var diff = Math.Abs(expectedMatrix[i, j] - matrix[i, j]);
                Assert.That(diff, Is.LessThanOrEqualTo(1e-4));
            }
        }
    }
}