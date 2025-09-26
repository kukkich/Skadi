namespace Skadi.LinearAlgebra.Vectors;

// Todo кажется IVector<T> излишен, т.к. set можно делать только лишь в обычный вектор
// Уже удалил IMatrix по такой же логике
public interface IVector<T> : IReadonlyVector<T>
{
    public new T this[int x] { set; }
}
