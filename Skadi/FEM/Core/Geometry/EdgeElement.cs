namespace Skadi.FEM.Core.Geometry;

public class EdgeElement : IEdgeElement
{
    public IReadOnlyList<int> EdgeIds { get; set; }
    public int AreaId { get; }
    public IReadOnlyList<int> NodeIds { get; }

    public EdgeElement(int areaId, IEnumerable<int> nodeIds, IEnumerable<int> edgeIds)
    {
        AreaId = areaId;
        EdgeIds = edgeIds.ToArray().AsReadOnly();
        NodeIds = nodeIds.ToArray().AsReadOnly();
    }

    public EdgeElement(int areaId, IEnumerable<int> nodeIds)
        : this(areaId, nodeIds, []) 
    { }
    
    public override string ToString()
    {
        return $"[{string.Join(", ", NodeIds)}] {{{string.Join(", ", EdgeIds)}}}";
    }
}