using Skadi.EquationsSystem;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.Algorithms.Splines;

public abstract class SplineEquationAssembler<TPoint>
(
    IPointsCollection<TPoint> nodes,
    ISplineStackLocalAssembler<IElement, TPoint> splineLocalAssembler,
    IStackLocalAssembler<IElement> localAssembler,
    IInserter<Matrix> inserter
)
{
    protected abstract int LocalMatrixSize { get; }
    protected readonly IPointsCollection<TPoint> Nodes = nodes;

    public void BuildEquation
    (
        Equation<Matrix> equation,
        FuncValue<TPoint>[] functionValues,
        IEnumerable<IElement> elements,
        double[]? weights = null
    )
    {
        var matrix = new MatrixSpan(stackalloc double[LocalMatrixSize * LocalMatrixSize], LocalMatrixSize);
        Span<double> vector = stackalloc double[LocalMatrixSize];
        var indexes = new StackIndexPermutation(stackalloc int[LocalMatrixSize]);

        for (var i = 0; i < functionValues.Length; i++)
        {
            var currentFunctionValue = functionValues[i];
            var currentWeight = WeightFactory(i);
            var element = elements.First(e => ElementHas(e, currentFunctionValue.Point));

            splineLocalAssembler.AssembleBasisFunctions(element);
            splineLocalAssembler.AssembleMatrix(element, currentFunctionValue.Point, currentWeight, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            inserter.InsertMatrix(equation.Matrix, localMatrix);

            splineLocalAssembler.AssembleRightSide(element, currentFunctionValue, currentWeight,  vector, indexes);
            var localRightSide = new StackLocalVector(vector, indexes);
            inserter.InsertVector(equation.RightSide, localRightSide);
        }

        foreach (var element in elements)
        {
            localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            inserter.InsertMatrix(equation.Matrix, localMatrix);
        }

        return;

        double WeightFactory(int index) => weights == null ? 1d : weights[index];
    }

    protected abstract bool ElementHas(IElement element, TPoint node);
}