namespace Skadi.FiniteElement.Core.Assembling.Params;

public interface IAreaProvider<out TArea>
{
    public TArea GetArea(int areaId);
}