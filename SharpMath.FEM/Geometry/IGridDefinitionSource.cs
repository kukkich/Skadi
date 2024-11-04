namespace SharpMath.FEM.Geometry;

public interface IGridDefinitionProvider<out TDefinition>
{
    public TDefinition Get();
}