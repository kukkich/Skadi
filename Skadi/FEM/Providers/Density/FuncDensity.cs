using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;

namespace Skadi.FEM.Providers.Density;

public class FuncDensity<TPoint, TResult> : INodeDefinedParameter<TResult>
{
    private readonly IPointsCollection<TPoint> _nodes;
    private readonly Func<TPoint, TResult> _func;

    public FuncDensity(IPointsCollection<TPoint> nodes, Func<TPoint, TResult> func)
    {
        _nodes = nodes;
        _func = func;
    }
    
    public TResult Get(int nodeIndex)
    {
        var node = _nodes[nodeIndex];
        return _func(node);
    }
}