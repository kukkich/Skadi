namespace Skadi.FEM.Core.Geometry;

public interface IGridBuilder<TPoint, in TDefinition, TElement>
    where TElement : IElement
{
    public Grid<TPoint, TElement> Build(TDefinition definition);
}