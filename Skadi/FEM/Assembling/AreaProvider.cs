using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Assembling;

public class AreaProvider<TArea>(TArea[] areas) : IAreaProvider<TArea>
{
    public TArea GetArea(int areaId) => areas[areaId];
}