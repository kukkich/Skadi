namespace Skadi.EquationsSystem.Preconditions.Null;

public class NullPreconditionerFactory<TMatrix> : IExtendedPreconditionerFactory<TMatrix>
{
    public IPreconditioner CreatePreconditioner(TMatrix matrix) => Create(matrix).Item1;

    public (IPreconditioner, IPreconditionerPart) Create(TMatrix matrix)
    {
        return (NullPreconditioner.Instance, NullPreconditioner.Instance);
    }
}