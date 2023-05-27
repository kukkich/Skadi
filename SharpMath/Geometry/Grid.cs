using System.Xml.Linq;

namespace SharpMath.Geometry;

public class Grid<TPoint, TElement>
{
    public TPoint[] Nodes { get; }
    public TElement[] Elements { get; }

    public Grid(
        IEnumerable<TPoint> nodes,
        IEnumerable<TElement> elements
    )
    {
        Nodes = nodes.ToArray();
        Elements = elements.ToArray();
    }
}