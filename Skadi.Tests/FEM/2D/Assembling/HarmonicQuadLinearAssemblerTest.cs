using System.Numerics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Skadi.Algorithms.Integration;
using Skadi.EquationsSystem;
using Skadi.FEM._2D.Assembling;
using Skadi.FEM._2D.BasisFunctions;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Deprecated._2D.Assembling;
using Skadi.FEM.Deprecated.Core.Harmonic_OLD;
using Skadi.FEM.Materials.HarmonicWithoutChi;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Vector = Skadi.LinearAlgebra.Vectors.Vector;

// ReSharper disable InconsistentNaming

namespace Skadi.Tests.FEM._2D.Assembling;

public class HarmonicQuadLinearAssemblerTest
{
    private HarmonicQuadLinearAssembler assembler = null!;
    private HarmonicLocalAssembler verifiedAssembler = null!;

    [SetUp]
    public void Setup()
    {
        var points = new IrregularPointsCollection<Vector2D>([
            new Vector2D(10, -20),
            new Vector2D(30, -20),
            new Vector2D(10, -5),
            new Vector2D(30, -5)
        ]);
        var basicFunctionsProvider = new QuadLinearNonScaledFunctions2DProvider();

        var areaMock = new Mock<IAreaProvider<AreaDefinition>>();
        areaMock.Setup(x => x.GetArea(It.IsAny<int>()))
            .Returns(() => new AreaDefinition(0, 1, 0, 1));
        var materialMock = new Mock<IMaterialProvider<Material>>();
        materialMock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns(() => new Material(Physics.Mu, 1e-1));
        var densityMock = new Mock<INodeDefinedParameter<Complex>>();
        densityMock.Setup(x => x.Get(It.IsAny<int>()))
            .Returns(() => new Complex(-2.5, 1.7));
        var frequencyMock = new Mock<IConstantProvider<double>>();
        frequencyMock.Setup(x => x.Get())
            .Returns(() => 9d * Math.PI);

        assembler = new HarmonicQuadLinearAssembler(
            points,
            areaMock.Object,
            new Gauss2D(GaussConfig.Gauss2(1), NullLogger.Instance),
            materialMock.Object,
            basicFunctionsProvider,
            basicFunctionsProvider,
            densityMock.Object,
            frequencyMock.Object
        );
        verifiedAssembler = new HarmonicLocalAssembler(new HarmonicContext<Vector2D, IElement, SparseMatrix>()
        {
            DensityFunctionProvider = densityMock.Object,
            Equation = new Equation<SparseMatrix>(new SparseMatrix([0], [0]), Vector.Create(1), Vector.Create(1)),
            FirstConditions = [],
            Frequency = frequencyMock.Object.Get(),
            Grid = new Grid<Vector2D, IElement>(points, []),
            Materials = materialMock.Object,
            SecondConditions = []
        });
    }

    [Test]
    public void MatrixIndexesPermutationShouldBeCorrect()
    {
        var element = new Element(0, [0, 1, 2, 3]);

        var matrix = new MatrixSpan(stackalloc double[8 * 8], 8);
        var indexesExpected = new StackIndexPermutation(stackalloc int[8]);
        var indexesActual = new StackIndexPermutation(stackalloc int[8]);

        verifiedAssembler.AssembleMatrix(element, matrix, indexesExpected);
        assembler.AssembleMatrix(element, matrix, indexesActual);

        for (var i = 0; i < 8; i++)
        {
            Assert.That(indexesExpected.Apply(i), Is.EqualTo(indexesActual.Apply(i)));
        }
    }

    [Test]
    public void VectorIndexesPermutationShouldBeCorrect()
    {
        var element = new Element(0, [0, 1, 2, 3]);

        Span<double> vector = stackalloc double[8];
        var indexesExpected = new StackIndexPermutation(stackalloc int[8]);
        var indexesActual = new StackIndexPermutation(stackalloc int[8]);

        verifiedAssembler.AssembleRightSide(element, vector, indexesExpected);
        assembler.AssembleRightSide(element, vector, indexesActual);

        for (var i = 0; i < 8; i++)
        {
            Assert.That(indexesExpected.Apply(i), Is.EqualTo(indexesActual.Apply(i)));
        }
    }

    [Test]
    public void MatrixShouldBeCorrect()
    {
        var element = new Element(0, [0, 1, 2, 3]);

        var matrixExpected = new MatrixSpan(stackalloc double[8 * 8], 8);
        var matrixActual = new MatrixSpan(stackalloc double[8 * 8], 8);
        var indexes = new StackIndexPermutation(stackalloc int[8]);

        verifiedAssembler.AssembleMatrix(element, matrixExpected, indexes);
        assembler.AssembleMatrix(element, matrixActual, indexes);

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                var diff = Math.Abs(matrixActual[i, j] - matrixExpected[i, j]);
                Assert.That(diff, Is.LessThanOrEqualTo(1e-10));
            }
        }
    }

    [Test]
    public void VectorShouldBeCorrect()
    {
        var element = new Element(0, [0, 1, 2, 3]);

        Span<double> vectorExpected = stackalloc double[8];
        Span<double> vectorActual = stackalloc double[8];
        var indexes = new StackIndexPermutation(stackalloc int[8]);

        verifiedAssembler.AssembleRightSide(element, vectorExpected, indexes);
        assembler.AssembleRightSide(element, vectorActual, indexes);

        for (var i = 0; i < 8; i++)
        {
            var diff = Math.Abs(vectorActual[i] - vectorExpected[i]);
            Assert.That(diff, Is.LessThanOrEqualTo(1e-10));
        }
    }
}