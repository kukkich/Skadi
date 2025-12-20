namespace ComplexSolvers;

public class Progress<T>(Action<T> action) : IProgress<T>
{
    public void Report(T value) => action(value);
}