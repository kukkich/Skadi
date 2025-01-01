using Microsoft.Extensions.Logging.Abstractions;
using Skadi.FEM._2D.Assembling;
using Skadi.FEM._2D.BasisFunctions;
using Skadi.FEM.Assembling;
using Skadi.FEM.Core;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Materials.LambdaGamma;
using Skadi.FEM.Materials.Providers;
using Skadi.FEM.Providers.Density;
using Skadi.Geometry._2D;
using Skadi.Integration;
using Skadi.Matrices;

namespace Skadi.Tests.FEM._2D.Assembling;

public class QuadLinearAssembler2DTest
{
    private QuadLinearAssembler2D assembler = null!;
    
    [SetUp]
    public void Setup()
    {
        var points = new IrregularPointsCollection<Point2D>([
            new Point2D(1, 1),
            new Point2D(5, 3),
            new Point2D(2, 5),
            new Point2D(4, 5)
        ]);
        var basicFunctionsProvider = new QuadLinearNonScaledFunctions2DProvider();
        assembler = new QuadLinearAssembler2D(
            points,
            new AreaProvider<AreaDefinition>([new AreaDefinition(0, 1, 0, 1)]),
            new Gauss2D(GaussConfig.Gauss3(1), NullLogger.Instance),
            new FromArrayMaterialProvider<Material>([new Material(1, 0)]),
            basicFunctionsProvider,
            basicFunctionsProvider,
            new FuncDensity<Point2D, double>(points, x => 0)
        );
    }

    [Test]
    public void StiffnessShouldBeCorrect()
    {
        var element = new Element(0, [0, 1, 2, 3]);
        var expectedMatrix = new Matrix(new double[,]
        {
            {0.3982285392, -0.04645707836, -0.1205713479, -0.2312001129 },
            {-0.04645707836, 0.8429141567, -0.5088573041, -0.2875997743 },
            {-0.1205713479, -0.5088573041, 0.9264283699, -0.2969997178 },
            {-0.2312001129, -0.2875997743, -0.2969997178, 0.8157996049 },
        });
        
        Span<double> matrixValues = stackalloc double[4 * 4];
        Span<int> permutations = [0, 1, 2, 3];
        var matrix = new StackMatrix(matrixValues, 4);
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