using Skadi.Geometry.Shapes;

namespace Skadi.Geometry.Splitting;

public readonly record struct ProportionalSplitter : ICurveSplitter
{
    public int Steps { get; }
    public double DischargeRatio { get; }
    private readonly double _lengthCoefficient;

    public ProportionalSplitter(int steps, double dischargeRatio)
    {
        if (Math.Abs(dischargeRatio - 1d) < 1e-5)
            throw new NotSupportedException();
        Steps = steps;
        DischargeRatio = dischargeRatio;
        _lengthCoefficient = (DischargeRatio - 1d) / (Math.Pow(DischargeRatio, steps) - 1d);
    }
    
    public IEnumerable<TPoint> EnumerateValues<TPoint>(IParametricCurve<TPoint> parametricCurve)
    {
        var step = _lengthCoefficient;

        var stepNumber = 0;
        var t = 0d;

        while (stepNumber < Steps)
        {
            yield return parametricCurve.GetByParameter(t);
            var tNext = step * (Math.Pow(DischargeRatio, stepNumber + 1) - 1d) / (DischargeRatio - 1d);

            if (t == tNext)
            {
                throw new Exception($"Следующий шаг разбиения привел к тому же значению {t}");
            }

            t = tNext;
            stepNumber++;
        }

        yield return parametricCurve.GetByParameter(1d);
    }
}