using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Skadi;
using Skadi.Matrices;
using Skadi.Vectors;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace SkadiBenchmarks;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
// Похоже, что нет разницы между использованием Vector и Span (и ReadOnly модификаций) 
public class MatrixVectorMultiplicationBenchmark
{
    private Vector _vector;
    private Matrix _matrix;
    private MatrixBase _matrixBase;
    private Vector _resultVector;

    private double[] _stackMatrix;
    private Vector _stackVector;
    private Vector _resultStackVector;

    [Params(100, 500, 5000, 10000)] 
    public int Size { get; set; }

    [GlobalSetup]
    public void SetUp()
    {
        var (min, max) = (-10d, 10d);
        var random = new Random(12039120);

        _stackMatrix = Vector.Create(Size * Size, _ => MatrixFactory()).AsSpan()
            .ToArray();
        _vector = Vector.Create(Size, _ => VectorFactory());
        _stackVector = Vector.Create(Size, _ => min + random.NextDouble() * (max - min));
        _resultVector = Vector.Create(Size);
        _resultStackVector = Vector.Create(Size);

        var matrix = new double[Size, Size];
        var matrixBase = new double[Size, Size];
        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < Size; j++)
            {
                matrix[i, j] = MatrixFactory();
                matrixBase[i, j] = MatrixFactory();
            }
        }

        _matrix = new Matrix(matrix);
        _matrixBase = new Matrix(matrixBase);

        return;

        double VectorFactory() => min + random.NextDouble() * (max - min);

        double MatrixFactory() => min / 10 + random.NextDouble() * (max / 10 - min / 10);
    }

    [Benchmark]
    public Vector ExplicitMultiplication()
    {
        return LinAl.Multiply(_matrix, _vector, _resultVector);
    }

    [Benchmark]
    public Vector ExplicitBaseMatrixMultiplication()
    {
        return LinAl.Multiply(_matrixBase, _vector, _resultVector);
    }

    [Benchmark]
    public Span<double> SpanMultiplication()
    {
        return LinAl.Multiply(new MatrixSpan(_stackMatrix, Size), _vector, _resultVector);
    }

    [Benchmark(Baseline = true)]
    public Span<double> SpanWitReadonlyMatrixMultiplication()
    {
        return LinAl.Multiply(new ReadOnlyMatrixSpan(_stackMatrix, Size), _stackVector, _resultStackVector);
    }
}