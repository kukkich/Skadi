namespace Skadi.FiniteElement.Core.Assembling.Params;

public interface IUniversalParameterProvider<in T, out TResult>
{
    public TResult Get(T parameter);
}