namespace Skadi.FEM.Core;

public class Element : IElement
{
    public int AreaId { get; }
    public IReadOnlyList<int> NodeIds { get; }

    public Element(int areaId, IEnumerable<int> nodeIds)
    {
        AreaId = areaId;
        NodeIds = nodeIds.ToArray().AsReadOnly();
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", NodeIds)}]";
    }
}