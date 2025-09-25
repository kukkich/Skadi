﻿using Skadi.EquationsSystem;
using Skadi.LinearAlgebra.Matrices.Sparse;

namespace Skadi.FEM.Core.Assembling.Boundary.First;

public class GaussExcluderSparse : IFirstBoundaryApplier<SparseMatrix>
{
    public void Apply(Equation<SparseMatrix> equation, FirstCondition condition)
    {
        var value = condition.Value;
        var nodeIndex = condition.NodeIndex;
        equation.RightSide[nodeIndex] = value;
        equation.Matrix.Diagonal[nodeIndex] = 1d;

        // todo may be replaced by row iteration
        for (var j = equation.Matrix.RowsIndexes[nodeIndex];
             j < equation.Matrix.RowsIndexes[nodeIndex + 1];
             j++)
        {
            equation.Matrix.LowerValues[j] = 0d;
        }

        for (var j = nodeIndex + 1; j < equation.Matrix.RowsCount; j++)
        {
            var elementIndex = equation.Matrix[j, nodeIndex];

            if (elementIndex == -1) continue;
            equation.Matrix.UpperValues[elementIndex] = 0;
        }
    }
}