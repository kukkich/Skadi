using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM._1D.Assembling;

public class LagrangeCubicAssembler1D(IPointsCollection<double> nodes, double alpha) : IStackLocalAssembler<IElement>
{
    private static readonly double[] StiffnessMatrixValues =
    [
        148, -189, 54, -13,
        -189, 432, -297, 54,
        54, -297, 432, -189,
        -13, 54, -189, 148
    ];
    private static ReadOnlyMatrixSpan StiffnessMatrix => new(StiffnessMatrixValues, 4);

    public void AssembleMatrix(IElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes)
    {
        var left = nodes[element.NodeIds[0]];
        var right = nodes[element.NodeIds[1]];
        var length = right - left;
        
        var k = alpha / (40d * length);
        
        LinAl.Multiply(k, StiffnessMatrix, matrixSpan);

        var leftNodeId = element.NodeIds[0];
        for (var i = 0; i < 4; i++)
        {
            indexes.Permutation[i] = leftNodeId * 3 + i;
        }
    }

    public void AssembleRightSide(IElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        throw new NotImplementedException();
    }
}