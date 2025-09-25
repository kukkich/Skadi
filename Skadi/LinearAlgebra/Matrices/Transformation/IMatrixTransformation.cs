using Skadi.LinearAlgebra.Vectors;

namespace Skadi.LinearAlgebra.Matrices.Transformation;

public interface IMatrixTransformation
{
    public Vector SolveEqualTo(Vector rightSide);
}