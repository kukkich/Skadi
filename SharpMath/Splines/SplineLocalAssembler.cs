using SharpMath.FEM.Core;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;

namespace SharpMath.Splines;

public class SplineLocalAssembler : ISplineStackLocalAssembler<IElement, Point2D>
{
    private readonly IBasisFunctionsProvider<IElement, Point2D> _basisFunctionsProvider;
    private IBasisFunction<Point2D>[] _basisFunctions;

    public SplineLocalAssembler(IBasisFunctionsProvider<IElement, Point2D> basisFunctionsProvider)
    {
        _basisFunctionsProvider = basisFunctionsProvider;
    }

    public void AssembleBasisFunctions(IElement element)
    {
        _basisFunctions = _basisFunctionsProvider.GetFunctions(element);
    }

    public void AssembleMatrix(IElement element, Point2D point, double weight, StackMatrix matrix, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIds.Count * 4; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                matrix[i, j] = weight * _basisFunctions[i].Evaluate(point) * _basisFunctions[j].Evaluate(point);
                matrix[j, i] = matrix[i, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(IElement element, FuncValue functionValue, double weight, Span<double> vector, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIds.Count * 4; i++)
        {
            vector[i] = weight * _basisFunctions[i].Evaluate(functionValue.Point) * functionValue.Value;
        }

        FillIndexes(element, indexes);
    }

    private static void FillIndexes(IElement element, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                indexes.Permutation[i * 4 + j] = element.NodeIds[i] * 4 + j;
            }
        }
    }
}