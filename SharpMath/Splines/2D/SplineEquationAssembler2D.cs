using SharpMath.FEM.Core;
using SharpMath.FEM.Geometry;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;

namespace SharpMath.Splines._2D;

public class SplineEquationAssembler2D : SplineEquationAssembler<Point2D>
{
    protected override int LocalMatrixSize => 16;
    
    public SplineEquationAssembler2D(
        IPointsCollection<Point2D> nodes, 
        ISplineStackLocalAssembler<IElement, Point2D> splineLocalAssembler, 
        IStackLocalAssembler<IElement> localAssembler, 
        IStackInserter<Matrix> inserter
    ) : base(nodes, splineLocalAssembler, localAssembler, inserter)
    { }

    protected override bool ElementHas(IElement element, Point2D node)
    {
        var leftBottom = Nodes[element.NodeIds[0]];
        var rightTop = Nodes[element.NodeIds[^1]];
    
        return leftBottom.X <= node.X && node.X <= rightTop.X && 
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}