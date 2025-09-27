using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;

namespace Skadi.FEM.Providers.Density;

public class FuncDensity<TPoint, TResult>(IPointsCollection<TPoint> nodes, Func<TPoint, TResult> func)
    : INodeDefinedParameter<TResult>, IUniversalParameterProvider<TPoint, TResult>
{
    public TResult Get(int nodeId)
    {
        var node = nodes[nodeId];
        return func(node);
    }

    public TResult Get(TPoint node) => func(node);
}