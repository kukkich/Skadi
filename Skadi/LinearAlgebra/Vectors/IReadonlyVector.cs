using Skadi.LinearAlgebra.Matrices.Sparse.Storages;

namespace Skadi.LinearAlgebra.Vectors;

public interface IReadonlyVector<T> : IEnumerable<IndexValue<T>>
{
    public T this[int x] { get; }
    public int Length { get; }
    public T Norm { get; }

    public T ScalarProduct(IReadonlyVector<T> v);
}