namespace Skadi.FEM.Core.Geometry;

public interface IGridBuilder<TPoint, in TDefinition>
{
    public Grid<TPoint, IElement> Build(TDefinition definition);
}