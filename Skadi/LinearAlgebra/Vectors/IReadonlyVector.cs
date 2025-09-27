namespace Skadi.LinearAlgebra.Vectors;

public interface IReadonlyVector<T> : IReadOnlyList<T>
{
    public T Norm { get; }
    public T ScalarProduct(IReadonlyVector<T> v);
}