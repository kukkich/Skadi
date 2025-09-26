using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.Algorithms.Splines;

public abstract class SplineLocalAssembler<TPoint>(IBasisFunctionsProvider<IElement, TPoint> basisFunctionsProvider)
    : ISplineStackLocalAssembler<IElement, TPoint>
{
    private IBasisFunction<TPoint>[] _basisFunctions;

    public void AssembleBasisFunctions(IElement element)
    {
        _basisFunctions = basisFunctionsProvider.GetFunctions(element);
    }

    public void AssembleMatrix(IElement element, TPoint point, double weight, MatrixSpan matrixSpan, StackIndexPermutation indexes)
    {
        for (var i = 0; i < _basisFunctions.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                matrixSpan[i, j] = weight * _basisFunctions[i].Evaluate(point) * _basisFunctions[j].Evaluate(point);
                matrixSpan[j, i] = matrixSpan[i, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(IElement element, FuncValue<TPoint> functionValue, double weight, Span<double> vector, StackIndexPermutation indexes)
    {
        for (var i = 0; i < _basisFunctions.Length; i++)
        {
            vector[i] = weight * _basisFunctions[i].Evaluate(functionValue.Point) * functionValue.Value;
        }

        FillIndexes(element, indexes);
    }

    protected abstract void FillIndexes(IElement element, StackIndexPermutation indexes);
}