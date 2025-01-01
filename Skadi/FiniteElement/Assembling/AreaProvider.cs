using Skadi.FiniteElement.Core.Assembling.Params;

namespace Skadi.FiniteElement.Assembling;

public class AreaProvider<TArea> : IAreaProvider<TArea>
{
    private readonly TArea[] _areas;

    public AreaProvider(TArea[] areas)
    {
        _areas = areas;
    }

    public TArea GetArea(int areaId) => _areas[areaId];
}