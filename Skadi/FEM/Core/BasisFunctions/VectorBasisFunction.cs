using Skadi.Geometry._3D;

namespace Skadi.FEM.Core.BasisFunctions;

public class VectorBasisFunction<T>(Func<T, T> function, Func<T, Vector3D> curl) : IVectorBasicFunction<T>
{
    public T Evaluate(T point) => function(point);
    public Vector3D Curl(T point) => curl(point);
}