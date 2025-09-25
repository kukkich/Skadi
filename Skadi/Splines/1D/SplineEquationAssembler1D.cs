﻿using Skadi.FEM.Core;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.Matrices;

namespace Skadi.Splines._1D;

public class SplineEquationAssembler1D : SplineEquationAssembler<double>
{
    protected override int LocalMatrixSize => 4;

    public SplineEquationAssembler1D(
        IPointsCollection<double> nodes, 
        ISplineStackLocalAssembler<IElement, double> splineLocalAssembler, 
        IStackLocalAssembler<IElement> localAssembler, 
        IInserter<Matrix> inserter) 
        : base(nodes, splineLocalAssembler, localAssembler, inserter)
    { }

    protected override bool ElementHas(IElement element, double node)
    {
        var left = Nodes[element.NodeIds[0]];
        var right = Nodes[element.NodeIds[1]];
        
        return left <= node && node <= right;
    }
}