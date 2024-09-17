using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;

namespace SharpMath.Splines;

public class SplineLocalAssembler : ISplineStackLocalAssembler<BicubicFiniteElement, Point>
{
    private readonly IBasisFunctionsProvider<BicubicFiniteElement, Point> _basisFunctionsProvider;
    private IBasisFunction<Point>[] _basisFunctions;

    public SplineLocalAssembler(IBasisFunctionsProvider<BicubicFiniteElement, Point> basisFunctionsProvider)
    {
        _basisFunctionsProvider = basisFunctionsProvider;
    }

    public void AssembleBasisFunctions(BicubicFiniteElement element)
    {
        _basisFunctions = _basisFunctionsProvider.GetFunctions(element);
    }

    public void AssembleMatrix(BicubicFiniteElement element, Point point, double weight, StackMatrix matrix, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                matrix[i, j] = weight * _basisFunctions[i].Evaluate(point) * _basisFunctions[j].Evaluate(point);
                matrix[j, i] = matrix[i, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(BicubicFiniteElement element, FuncValue functionValue, double weight, Span<double> vector, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            vector[i] = weight * _basisFunctions[i].Evaluate(functionValue.Point) * functionValue.Value;
        }

        FillIndexes(element, indexes);
    }

    private static void FillIndexes(BicubicFiniteElement element, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            indexes.Permutation[i] = element.NodeIndexes[i];
        }
    }
}