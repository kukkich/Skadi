using Skadi.FEM.Core;
using Skadi.FEM.Core.Geometry;

namespace Skadi.Splines;

public class SplineContext<TPoint, TElement, TMatrix> 
    where TElement : IElement
{
    public required Grid<TPoint, TElement> Grid { get; set; }
    public required Equation<TMatrix> Equation { get; set; }
    public required FuncValue<TPoint>[] FunctionValues { get; set; }
    public required double[] Weights { get; set; }
    public required double Alpha { get; set; }
    public required double Beta { get; set; }
}