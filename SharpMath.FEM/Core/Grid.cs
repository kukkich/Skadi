using SharpMath.FEM.Geometry;

namespace SharpMath.FEM.Core;

public class Grid<TPoint, TElement> where TElement : IElement
{
    public IPointsCollection<TPoint> Nodes { get; }
    public TElement[] Elements { get; }

    public Grid(
        IPointsCollection<TPoint> nodes,
        IEnumerable<TElement> elements
    )
    {
        Nodes = nodes;
        Elements = elements.ToArray();
    }
}