using SharpMath.FEM.Geometry;
using SharpMath.FiniteElement.Core.Assembling.Params;

namespace SharpMath.FiniteElement.Providers.Density;

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