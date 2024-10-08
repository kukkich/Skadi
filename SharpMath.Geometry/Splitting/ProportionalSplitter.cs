using SharpMath.Geometry._2D;
using SharpMath.Geometry.Shapes;
using SharpMath.Primitives;

namespace SharpMath.Geometry.Splitting;

public class ProportionalSplitter : ICurveSplitter
{
    public Positive<int> Steps { get; }
    public double DischargeRatio { get; }
    private readonly double _lengthCoefficient;

    public ProportionalSplitter(Positive<int> steps, double dischargeRatio)
    {
        if (Math.Abs(dischargeRatio - 1d) < 1e-5)
            throw new NotSupportedException();
        Steps = steps;
        DischargeRatio = dischargeRatio;
        _lengthCoefficient = (DischargeRatio - 1d) / (Math.Pow(DischargeRatio, steps) - 1d);
    }
    
    public IEnumerable<TPoint> EnumerateValues<TPoint>(ICurve<TPoint> curve)
    {
        var step = _lengthCoefficient;

        var stepNumber = 0;
        var t = 0d;

        while (stepNumber < Steps)
        {
            yield return curve.GetByParameter(t);
            var tNext = step * (Math.Pow(DischargeRatio, stepNumber + 1) - 1d) / (DischargeRatio - 1d);

            if (t == tNext)
            {
                throw new Exception($"Следующий шаг разбиения привел к тому же значению {t}");
            }

            t = tNext;
            stepNumber++;
        }

        yield return curve.GetByParameter(1d);
    }
}