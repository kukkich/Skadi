using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Assembling;

public class AreaProvider<TArea> : IAreaProvider<TArea>
{
    private readonly TArea[] _areas;

    public AreaProvider(TArea[] areas)
    {
        _areas = areas;
    }

    public TArea GetArea(int areaId) => _areas[areaId];
}