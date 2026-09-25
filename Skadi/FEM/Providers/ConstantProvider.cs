using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Providers;

public class ConstantProvider<T>(T value) : IConstantProvider<T>
{
    public T Value { get; set; } = value;
    
    public T Get()
    {
        return Value;
    }
}