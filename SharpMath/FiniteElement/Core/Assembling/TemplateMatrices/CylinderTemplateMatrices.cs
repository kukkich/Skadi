using SharpMath.Matrices;

namespace SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;

public class CylinderTemplateMatrices
{
    public static ImmutableMatrix StiffnessR1D(double r, double elementEdgeSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                { 1d, -1d },
                { -1d, 1d },
            },
            (2 * r + elementEdgeSize) / (2 * elementEdgeSize)
        );
    }

    public static ImmutableMatrix StiffnessZ1D(double elementEdgeSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                { 1d, -1d },
                { -1d, 1d },
            },
            1 / elementEdgeSize
        );
    }

    public static ImmutableMatrix MassR1D(double r, double elementEdgeSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                {r * 2d + elementEdgeSize / 2, r + elementEdgeSize / 2},
                {r + elementEdgeSize / 2, r * 2d + elementEdgeSize / 2 * 3},
            },
            elementEdgeSize / 6
        );
    }

    public static ImmutableMatrix MassZ1D(double elementEdgeSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                { 2d, 1d },
                { 1d, 2d },
            },
            elementEdgeSize / 6
        );
    }
}