namespace Skadi.FEM.Core.Geometry;

public class EdgeElement(int areaId, IEnumerable<int> nodeIds, IEnumerable<int> edgeIds) : IEdgeElement
{
    public IReadOnlyList<int> EdgeIds { get; set; } = edgeIds.ToArray().AsReadOnly();
    public int AreaId { get; } = areaId;
    public IReadOnlyList<int> NodeIds { get; } = nodeIds.ToArray().AsReadOnly();

    public EdgeElement(int areaId, IEnumerable<int> nodeIds)
        : this(areaId, nodeIds, []) 
    { }
    
    public override string ToString()
    {
        return $"[{string.Join(", ", NodeIds)}] {{{string.Join(", ", EdgeIds)}}}";
    }
}