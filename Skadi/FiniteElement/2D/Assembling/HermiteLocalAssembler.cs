using Skadi.FEM.Core;
using Skadi.FEM.Geometry;
using Skadi.Geometry._2D;
using Skadi.FiniteElement.Core.Assembling;
using Skadi.FiniteElement.Core.Assembling.TemplateMatrices;
using Skadi.Matrices;

namespace Skadi.FiniteElement._2D.Assembling;

public class HermiteLocalAssembler : IStackLocalAssembler<IElement>
{
    private readonly IPointsCollection<Point2D> _nodes; // для GetSizes
    private readonly double _alpha;

    public HermiteLocalAssembler(IPointsCollection<Point2D> nodes, double alpha)
    {
        _nodes = nodes;
        _alpha = alpha;
    }

    public void AssembleMatrix(IElement element, StackMatrix matrix, StackIndexPermutation indexes)
    {
        var (width, lenght) = GetSizes(element);

        var stiffnessMatrixX = HermiteTemplateMatrices.HermiteStiffness1D(width);
        var stiffnessMatrixY = HermiteTemplateMatrices.HermiteStiffness1D(lenght);

        var massMatrixX = HermiteTemplateMatrices.HermiteMass1D(width);
        var massMatrixY = HermiteTemplateMatrices.HermiteMass1D(lenght);
        
        for (var i = 0; i < element.NodeIds.Count * 4; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                matrix[i, j] = _alpha * 
                               (stiffnessMatrixX[Mu(i), Mu(j)] * massMatrixY[Nu(i), Nu(j)] + 
                                massMatrixX[Mu(i), Mu(j)] * stiffnessMatrixY[Nu(i), Nu(j)]);
                matrix[j, i] = matrix[i, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(IElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        throw new NotImplementedException();
    }

    private static void FillIndexes(IElement element, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                indexes.Permutation[i * 4 + j] = element.NodeIds[i] * 4 + j;
            }
        }
    }

    private static int Mu(int i)
    {
        return i % 4;
    }

    private static int Nu(int i)
    {
        return i / 4;
    }
    
    private (double Width, double Length) GetSizes(IElement element)
    {
        throw new NotImplementedException("Замена для element.Width и element.Length");
    }
}