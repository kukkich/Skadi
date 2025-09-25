using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Matrices.Sparse.Decompositions;

namespace Skadi.EquationsSystem.Preconditions.LU;

public class LUPreconditionerCSRFactory : IExtendedPreconditionerFactory<CSRMatrix>
{
    public IPreconditioner CreatePreconditioner(CSRMatrix matrix) => Create(matrix).Item1;

    public (IPreconditioner, IPreconditionerPart) Create(CSRMatrix matrix)
    {
        var decomposed = IncompleteLU.Decompose(matrix);
        var preconditioner = new LUPreconditionerCSR(decomposed);
        return (preconditioner, preconditioner.GetPart());
    }
}