using SharpMath.Matrices;
using System.Reflection.Metadata.Ecma335;

namespace SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;

public static class HermiteTemplateMatrices
{
    public static ImmutableMatrix Stiffness1D(double elementEdgeSize)
    {
        return new ImmutableMatrix
            (
                new[,]
                {
                    { 36, 3 * elementEdgeSize, -36, 3 * elementEdgeSize },
                    { 3 * elementEdgeSize, 4 * Math.Pow(elementEdgeSize, 2), -3 * elementEdgeSize, -Math.Pow(elementEdgeSize, 2) },
                    { -36, -3 * elementEdgeSize, 36, -3 * elementEdgeSize },
                    { 3 * elementEdgeSize, -Math.Pow(elementEdgeSize, 2), -3 * elementEdgeSize, 4 * Math.Pow(elementEdgeSize, 2) },
                }, 
                30 * elementEdgeSize
            );
    }

    public static ImmutableMatrix Mass1D(double elementEdgeSize)
    {
        return new ImmutableMatrix
            (
                new[,]
                {
                    { 156, 22 * elementEdgeSize, 54, -13 * elementEdgeSize },
                    { 22 * elementEdgeSize, 4 * Math.Pow(elementEdgeSize, 2), 13 * elementEdgeSize, -3 * Math.Pow(elementEdgeSize, 2) },
                    { 54, 13 * elementEdgeSize, 156, -22 * elementEdgeSize },
                    { -13 * elementEdgeSize, -3 * Math.Pow(elementEdgeSize, 2), -22 * elementEdgeSize, 4 * Math.Pow(elementEdgeSize, 2) },
                },
                elementEdgeSize / 420
            );
    }
}