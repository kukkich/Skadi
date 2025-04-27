using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;

namespace Skadi.FEM.Providers.Density;

public class FuncDensity<TPoint, TResult> : INodeDefinedParameter<TResult>,
    IUniversalParameterProvider<TPoint, TResult>
{
    private readonly IPointsCollection<TPoint> _nodes;
    private readonly Func<TPoint, TResult> _func;

    public FuncDensity(IPointsCollection<TPoint> nodes, Func<TPoint, TResult> func)
    {
        _nodes = nodes;
        _func = func;
    }
    
    public TResult Get(int nodeId)
    {
        var node = _nodes[nodeId];
        return _func(node);
    }

    public TResult Get(TPoint node) => _func(node);
}