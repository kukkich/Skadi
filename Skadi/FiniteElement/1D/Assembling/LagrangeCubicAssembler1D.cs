using Skadi.FEM.Core;
using Skadi.FEM.Geometry;
using Skadi.FiniteElement.Core.Assembling;
using Skadi.Matrices;

namespace Skadi.FiniteElement._1D.Assembling;

public class LagrangeCubicAssembler1D : IStackLocalAssembler<IElement>
{
    private readonly IPointsCollection<double> _nodes;
    private readonly double _alpha;
    private static readonly Matrix StiffnessMatrix = new (new double[,]
    {
        {148, -189, 54, -13},
        {-189, 432, -297, 54},
        {54, -297, 432, -189},
        {-13, 54, -189, 148},
    });
    
    public LagrangeCubicAssembler1D(IPointsCollection<double> nodes, double alpha)
    {
        _nodes = nodes;
        _alpha = alpha;
    }

    public void AssembleMatrix(IElement element, StackMatrix matrix, StackIndexPermutation indexes)
    {
        var left = _nodes[element.NodeIds[0]];
        var right = _nodes[element.NodeIds[1]];
        var length = right - left;
        
        var k = _alpha / (40d * length);


        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                matrix[i, j] = k *  StiffnessMatrix[i, j];
            }
        }

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