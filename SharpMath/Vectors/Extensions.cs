namespace SharpMath.Vectors;

public static class Extensions
{
    public static bool IsAnyNaN(this IReadonlyVector<double> vector)
    {
        for (var i = 0; i < vector.Length; i++)
        {
            if (double.IsNaN(vector[i]))
            {
                return true;
            }
        }

        return false;
    }

    public static void Nullify(this IVector<double> vector)
    {
        for (var i = 0; i < vector.Length; i++)
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
    
    public static IEnumerable<T> WithNoIndexes<T>(this IReadonlyVector<T> source)
    {
        for (var i = 0; i < source.Length; i++)
        {
            yield return source[i];
        }
    }
}