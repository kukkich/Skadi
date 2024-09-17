using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;
using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.Splines;

namespace SharpMath.FiniteElement._2D.Assembling;

public class HermiteLocalAssembler : IStackLocalAssembler<BicubicFiniteElement>
{
    private readonly SplineContext<Point, BicubicFiniteElement, SymmetricSparseMatrix> _context;

    public HermiteLocalAssembler(SplineContext<Point, BicubicFiniteElement, SymmetricSparseMatrix> context)
    {
        _context = context;
    }

    public void AssembleMatrix(BicubicFiniteElement element, StackMatrix matrix, StackIndexPermutation indexes)
    {
        var stiffnessMatrixX = HermiteTemplateMatrices.HermiteStiffness1D(element.Width);
        var stiffnessMatrixY = HermiteTemplateMatrices.HermiteStiffness1D(element.Length);

        var massMatrixX = HermiteTemplateMatrices.HermiteMass1D(element.Width);
        var massMatrixY = HermiteTemplateMatrices.HermiteMass1D(element.Length);
        
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                matrix[i, j] = _context.Alpha * 
                               (stiffnessMatrixX[Mu(i), Mu(j)] * massMatrixY[Nu(i), Nu(j)] + 
                               massMatrixX[Mu(i), Mu(j)] * stiffnessMatrixY[Nu(i), Nu(j)]);
                matrix[j, i] = matrix[i, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(BicubicFiniteElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        throw new NotImplementedException();
    }

    private static void FillIndexes(BicubicFiniteElement element, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            indexes.Permutation[i] = element.NodeIndexes[i];
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
}