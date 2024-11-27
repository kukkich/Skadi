using Microsoft.Extensions.Logging.Abstractions;
using SharpMath.FEM.Core;
using SharpMath.FEM.Geometry._2D;
using SharpMath.FEM.Geometry._2D.Quad;
using SharpMath.FiniteElement._2D.Assembling;
using SharpMath.FiniteElement._2D.BasisFunctions;
using SharpMath.FiniteElement.Assembling;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Materials.LambdaGamma;
using SharpMath.FiniteElement.Materials.Providers;
using SharpMath.Geometry._2D;
using SharpMath.Integration;
using SharpMath.Matrices;

namespace SharpMath.Tests.FEM._2D.Assembling;

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
            new Gauss2D(GaussConfig.Gauss4(1), NullLogger.Instance),
            new DefaultMaterialProvider<Material>(),
            basicFunctionsProvider,
            basicFunctionsProvider
        );
    }

    [Test]
    [Ignore("")]
    public void StiffnessShouldBeCorrect()
    {
        var element = new Element(0, [0, 1, 2, 3]);

        Span<double> matrixValues = stackalloc double[4 * 4];
        var matrix = new StackMatrix(matrixValues, 4);
        var permutation = new StackIndexPermutation();
        
        assembler.AssembleMatrix(element, matrix, permutation);
        
        Assert.Fail();
    }
}