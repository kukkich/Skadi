using Skadi.FEM._1D.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Geometry.Shapes.Primitives;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.Algorithms.Splines._1D.CubicLagrange;

public class LagrangeSpline : ISpline<double>, IParametricCurve2D
{
    private static readonly double[,] Alpha =
    {
        { -4.5, 9.0, -5.5, 1.0 }, // psi0
        { 13.5, -22.5, 9.0, 0.0 }, // psi1
        { -13.5, 18.0, -4.5, 0.0 }, // psi2
        { 4.5, -4.5, 1.0, 0.0 } // psi3
    };

    private readonly LagrangeCubicFunction1DProvider _basisFunctionsProvider;
    private readonly Grid<double, IElement> _grid;
    private readonly Vector _weights;

    public Vector2D Start { get; }
    public Vector2D End { get; }

    public LagrangeSpline
    (
        LagrangeCubicFunction1DProvider basisFunctionsProvider,
        Grid<double, IElement> grid,
        Vector weights
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        _weights = weights;

        var left = grid.Nodes[grid.Elements[0].NodeIds[0]];
        var right = grid.Nodes[grid.Elements[^1].NodeIds[1]];

        Start = new Vector2D(left, Calculate(left));
        End = new Vector2D(right, Calculate(right));
    }

    public (double min, double max) MinMax()
    {
        var (_, elements) = _grid;

        var globalMin = double.PositiveInfinity;
        var globalMax = double.NegativeInfinity;
        const double eps = 1e-14;

        Span<double> beta = stackalloc double[4];
        Span<double> candidates = stackalloc double[4]; // максимум 2 корня + концы
        Span<double> Q = stackalloc double[4];

        foreach (var element in elements)
        {
            var leftIndex = element.NodeIds[0];

            // получаем локальные веса
            for (var j = 0; j < 4; j++)
            {
                var id = leftIndex * 3 + j;
                Q[j] = _weights[id];
            }

            // вычисляем коэффициенты f(t) = β₃ t³ + β₂ t² + β₁ t + β₀
            beta.Nullify();
            for (var j = 0; j < 4; j++)
            {
                beta[3] += Q[j] * Alpha[j, 0];
                beta[2] += Q[j] * Alpha[j, 1];
                beta[1] += Q[j] * Alpha[j, 2];
                beta[0] += Q[j] * Alpha[j, 3];
            }

            // производная f'(t) = 3*β₃ t² + 2*β₂ t + β₁
            var A = 3 * beta[3];
            var B = 2 * beta[2];
            var C = beta[1];

            var candidateCount = 0;
            candidates[candidateCount++] = 0.0; // t=0
            candidates[candidateCount++] = 1.0; // t=1

            // решаем квадратное уравнение для производной
            if (Math.Abs(A) > eps)
            {
                var D = B * B - 4 * A * C;
                if (D >= 0)
                {
                    var sqrtD = Math.Sqrt(D);
                    var t1 = (-B - sqrtD) / (2 * A);
                    var t2 = (-B + sqrtD) / (2 * A);
                    if (t1 is >= 0 and <= 1) 
                        candidates[candidateCount++] = t1;
                    if (t2 is >= 0 and <= 1) 
                        candidates[candidateCount++] = t2;
                }
            }
            else if (Math.Abs(B) > eps)
            {
                var t = -C / B;
                if (t is >= 0 and <= 1) 
                    candidates[candidateCount++] = t;
            }

            // вычисляем значения сплайна в кандидатах
            for (var i = 0; i < candidateCount; i++)
            {
                var t = candidates[i];
                var val = beta[3] * t * t * t + 
                          beta[2] * t * t + 
                          beta[1] * t + 
                          beta[0];
                globalMin = Math.Min(globalMin, val);
                globalMax = Math.Max(globalMax, val);
            }
        }

        return (globalMin, globalMax);
    }

    public static double DistanceBetween(LagrangeSpline f, LagrangeSpline g)
    {
        var (_, elements) = f._grid;

        var globalMin = double.PositiveInfinity;
        const double eps = 1e-14;

        // локальные разностные веса
        Span<double> Q = stackalloc double[4];
        // кандидаты значений t ∈ [0,1]
        Span<double> candidates = stackalloc double[4];
        Span<double> beta = stackalloc double[4];

        foreach (var element in elements)
        {
            var leftIndex = element.NodeIds[0];

            // вычисляем локальные веса разности f-g
            for (var j = 0; j < 4; j++)
            {
                var id = leftIndex * 3 + j;
                Q[j] = f._weights[id] - g._weights[id];
            }

            // коэффициенты кубического полинома d(t) = f(t) - g(t)
            beta.Nullify();
            for (var j = 0; j < 4; j++)
            {
                beta[3] += Q[j] * Alpha[j, 0];
                beta[2] += Q[j] * Alpha[j, 1];
                beta[1] += Q[j] * Alpha[j, 2];
                beta[0] += Q[j] * Alpha[j, 3];
            }

            // производная: d'(t) = 3*β3*t^2 + 2*β2*t + β1
            var A = 3 * beta[3];
            var B = 2 * beta[2];
            var C = beta[1];

            var candidatesCount = 0;
            candidates[candidatesCount++] = 0.0; // t=0
            candidates[candidatesCount++] = 1.0; // t=1

            // ищем экстремумы (корни производной)
            if (Math.Abs(A) > eps)
            {
                var D = B * B - 4 * A * C;
                if (D >= 0)
                {
                    var sqrtD = Math.Sqrt(D);
                    var inv = 1d / (2 * A);

                    var t1 = (-B - sqrtD) * inv;
                    var t2 = (-B + sqrtD) * inv;

                    if (t1 is >= 0 and <= 1)
                        candidates[candidatesCount++] = t1;
                    if (t2 is >= 0 and <= 1)
                        candidates[candidatesCount++] = t2;
                }
            }
            else if (Math.Abs(B) > eps)
            {
                var t = -C / B;
                if (t is >= 0 and <= 1)
                    candidates[candidatesCount++] = t;
            }

            // проверяем знак на концах интервала и экстремумах
            var hasSignChange = false;
            for (var i = 0; i < candidatesCount - 1; i++)
            {
                var t0 = candidates[i];
                var t1 = candidates[i + 1];

                var d0 = beta[3] * t0 * t0 * t0 + beta[2] * t0 * t0 + beta[1] * t0 + beta[0];
                var d1 = beta[3] * t1 * t1 * t1 + beta[2] * t1 * t1 + beta[1] * t1 + beta[0];

                if (d0 * d1 <= 0) // есть пересечение с нулём
                {
                    hasSignChange = true;
                    break;
                }
            }

            if (hasSignChange)
                return 0;

            // вычисляем |d(t)| для всех кандидатов и ищем минимум
            for (var i = 0; i < candidatesCount; i++)
            {
                var t = candidates[i];
                var d =
                    beta[3] * t * t * t +
                    beta[2] * t * t +
                    beta[1] * t +
                    beta[0];

                var abs = Math.Abs(d);
                if (abs < globalMin)
                    globalMin = abs;
            }
        }

        return globalMin;
    }

    public double Calculate(double point)
    {
        var element = _grid.Elements.First(e => ElementHas(e, point));
        var basisFunctions = _basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < 4; i++)
        {
            var weightId = element.NodeIds[0] * 3 + i;
            sum += _weights[weightId] * basisFunctions[i].Evaluate(point);
        }

        return sum;
    }

    private bool ElementHas(IElement element, double node)
    {
        var left = _grid.Nodes[element.NodeIds[0]];
        var right = _grid.Nodes[element.NodeIds[1]];

        var scale = Math.Max(Math.Abs(left), Math.Abs(right));
        scale = Math.Max(scale, Math.Abs(node));

        var eps = 1e-12 * scale;

        return left - eps <= node && node <= right + eps;
    }

    public Vector2D GetByParameter(CurveParameter t)
    {
        var x = Start.X + (End.X - Start.X) * t;
        var y = Calculate(x);
        return new Vector2D(x, y);
    }
}