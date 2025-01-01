namespace Skadi.FEM.Core.Geometry;

public interface IGridDefinitionProvider<out TDefinition>
{
    public TDefinition Get();
}