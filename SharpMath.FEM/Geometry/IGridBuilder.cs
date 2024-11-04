using SharpMath.FEM.Core;

namespace SharpMath.FEM.Geometry;

public interface IGridBuilder<TPoint, in TDefinition>
{
    public Grid<TPoint, IElement> Build(TDefinition definition);
}