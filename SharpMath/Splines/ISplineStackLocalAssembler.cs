using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.Matrices;

namespace SharpMath.Splines;

public interface ISplineStackLocalAssembler<in TElement, TPoint>
{
    public void AssembleBasisFunctions(TElement element);
    public void AssembleMatrix(TElement element, TPoint point, double weight, StackMatrix matrix, StackIndexPermutation indexes);
    public void AssembleRightSide(TElement element, FuncValue<TPoint> functionValue, double weight, Span<double> vector, StackIndexPermutation indexes);
}