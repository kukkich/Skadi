using Skadi.FEM.Core;

namespace Skadi.FEM.Geometry;

public interface IGridBuilder<TPoint, in TDefinition>
{
    public Grid<TPoint, IElement> Build(TDefinition definition);
}