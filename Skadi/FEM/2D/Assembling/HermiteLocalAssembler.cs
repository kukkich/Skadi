using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.TemplateMatrices;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM._2D.Assembling;

// nodes для GetSizes
public class HermiteLocalAssembler(IPointsCollection<Vector2D> nodes, double alpha) : IStackLocalAssembler<IElement>
{
    public void AssembleMatrix(IElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes)
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
                matrixSpan[i, j] = alpha * 
                               (stiffnessMatrixX[Mu(i), Mu(j)] * massMatrixY[Nu(i), Nu(j)] + 
                                massMatrixX[Mu(i), Mu(j)] * stiffnessMatrixY[Nu(i), Nu(j)]);
                matrixSpan[j, i] = matrixSpan[i, j];
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
        throw new NotImplementedException("Замена для element.Width и element.Count");
    }
}