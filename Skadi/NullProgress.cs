namespace Skadi;

public class NullProgress<T> : IProgress<T>
{
    public static readonly NullProgress<T> Instance = new();
    
    public void Report(T value) { }
}