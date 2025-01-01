using Skadi.Vectors;

namespace Skadi.Matrices.Transformation;

public interface IMatrixTransformation
{
    public Vector SolveEqualTo(Vector rightSide);
}