namespace Skadi.FEM.Core.Assembling.Params;

public interface IConstantProvider<out T>
{
    public T Get();
}