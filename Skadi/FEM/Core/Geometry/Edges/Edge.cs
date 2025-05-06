using System.Collections;

namespace Skadi.FEM.Core.Geometry.Edges;

public readonly record struct Edge : IEnumerable<int>
{
    public int Begin { get; }
    public int End { get; }

    public Edge(int node1, int node2)
    {
        Begin = int.Min(node1, node2);
        End = int.Max(node1, node2);
    }

    public IEnumerator<int> GetEnumerator()
    {
        yield return Begin;
        yield return End;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public override string ToString() => $"{Begin}<->{End}";
}