namespace Skadi.FEM.Core.Geometry;

public record Grid<TPoint, TElement>
(
    IPointsCollection<TPoint> Nodes,
    TElement[] Elements
) where TElement : IElement;