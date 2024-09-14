using SharpMath.Matrices;

namespace SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;

public static class TemplateHermiteMatrices
{
    public static Matrix HermiteStiffness1D(double length) => new Matrix(
            new double[,]
            {
                {1, 1},
                {1, 1},
            });
    
}