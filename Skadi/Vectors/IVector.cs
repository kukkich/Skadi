namespace Skadi.Vectors;

public interface IVector<T> : IReadonlyVector<T>
{
    public new T this[int x] { set; }
}
