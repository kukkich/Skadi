using Skadi.LinearAlgebra.Matrices.Sparse.Storages;

namespace Skadi.LinearAlgebra.Vectors;

public static class Extensions
{
    public static bool IsAnyNaN(this IReadonlyVector<double> vector)
    {
        return vector.Any(double.IsNaN);
    }

    public static void Nullify(this Vector vector)
    {
        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = 0;
        }
    }
    
    public static void Nullify(this Span<double> vector)
    {
        for (var i = 0; i < vector.Length; i++)
        {
            vector[i] = 0;
        }
    }
}

public static class EnumerableVectorExtensions
{
    public static Vector ToVector(this IEnumerable<double> source)
    {
        return new Vector(source.ToArray());
    }
    
    public static IEnumerable<IndexValue<T>> WithIndexes<T>(this IReadonlyVector<T> source)
    {
        return source.Select((t, i) => new IndexValue<T>(t, i));
    }
}