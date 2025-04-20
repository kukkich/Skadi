using Skadi.Matrices.Sparse;

namespace Skadi.EquationsSystem.Preconditions.LU;

public class LUPreconditionerCSRFactory : IExtendedPreconditionerFactory<CSRMatrix>
{
    public IPreconditioner CreatePreconditioner(CSRMatrix matrix) => Create(matrix).Item1;

    public (IPreconditioner, IPreconditionerPart) Create(CSRMatrix matrix)
    {
        throw new NotImplementedException();
    }
}