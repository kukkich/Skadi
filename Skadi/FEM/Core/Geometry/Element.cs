namespace Skadi.FEM.Core.Geometry;

public class Element(int areaId, IEnumerable<int> nodeIds) : IElement
{
    public int AreaId { get; } = areaId;
    public IReadOnlyList<int> NodeIds { get; } = nodeIds.ToArray().AsReadOnly();

    public override string ToString()
    {
        return $"[{string.Join(", ", NodeIds)}]";
    }
}