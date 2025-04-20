namespace Skadi.EquationsSystem.Preconditions;

public interface IPreconditionerFactory<in TMatrix>
{
    public IPreconditioner CreatePreconditioner(TMatrix matrix);
}

public interface IExtendedPreconditionerFactory<in TMatrix> : IPreconditionerFactory<TMatrix>
{
    public (IPreconditioner, IPreconditionerPart) Create(TMatrix matrix);
}