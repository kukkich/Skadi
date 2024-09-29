using SharpMath.FEM.Core;
using SharpMath.Geometry;

namespace SharpMath.FEM.Geometry;

public interface IGridBuilder<TPoint, in TDefinition>
{
    public Grid<TPoint, IElement> Build(TDefinition definition);
}