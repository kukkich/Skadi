using Skadi.Geometry._3D;

namespace Skadi.FEM.Core.BasisFunctions;

public class VectorBasisFunction<T> : IVectorBasicFunction<T>
{
    private readonly Func<T, T> _function;
    private readonly Func<T, Vector3D> _curl;

    public VectorBasisFunction(Func<T, T> function, Func<T, Vector3D> curl)
    {
        _function = function;
        _curl = curl;
    }

    public T Evaluate(T point) => _function(point);
    public Vector3D Curl(T point) => _curl(point);
}