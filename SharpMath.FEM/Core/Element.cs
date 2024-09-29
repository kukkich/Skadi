﻿namespace SharpMath.FEM.Core;

public class Element : IElement
{
    public int AreaId { get; }
    public IReadOnlyCollection<int> NodeIds { get; }

    public Element(int areaId, IEnumerable<int> nodeIds)
    {
        AreaId = areaId;
        NodeIds = nodeIds.ToList().AsReadOnly();
    }
}