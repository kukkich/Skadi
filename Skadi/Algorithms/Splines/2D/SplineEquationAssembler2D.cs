using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.Algorithms.Splines._2D;

public class SplineEquationAssembler2D(
    IPointsCollection<Vector2D> nodes,
    ISplineStackLocalAssembler<IElement, Vector2D> splineLocalAssembler,
    IStackLocalAssembler<IElement> localAssembler,
    IInserter<Matrix> inserter)
    : SplineEquationAssembler<Vector2D>(nodes, splineLocalAssembler, localAssembler, inserter)
{
    protected override int LocalMatrixSize => 16;

    protected override bool ElementHas(IElement element, Vector2D node)
    {
        var leftBottom = Nodes[element.NodeIds[0]];
        var rightTop = Nodes[element.NodeIds[^1]];
    
        return leftBottom.X <= node.X && node.X <= rightTop.X && 
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}