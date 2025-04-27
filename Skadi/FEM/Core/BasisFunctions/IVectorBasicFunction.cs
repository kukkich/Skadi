using Skadi.Geometry._3D;

namespace Skadi.FEM.Core.BasisFunctions;

public interface IVectorBasicFunction<TPoint>
{
    public TPoint Evaluate(TPoint point);
    public Vector3D Curl(TPoint point);
}