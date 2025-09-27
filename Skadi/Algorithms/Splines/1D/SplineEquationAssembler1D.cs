using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.Algorithms.Splines._1D;

public class SplineEquationAssembler1D(
    IPointsCollection<double> nodes,
    ISplineStackLocalAssembler<IElement, double> splineLocalAssembler,
    IStackLocalAssembler<IElement> localAssembler,
    IInserter<Matrix> inserter)
    : SplineEquationAssembler<double>(nodes, splineLocalAssembler, localAssembler, inserter)
{
    protected override int LocalMatrixSize => 4;

    protected override bool ElementHas(IElement element, double node)
    {
        var left = Nodes[element.NodeIds[0]];
        var right = Nodes[element.NodeIds[1]];
        
        return left <= node && node <= right;
    }
}