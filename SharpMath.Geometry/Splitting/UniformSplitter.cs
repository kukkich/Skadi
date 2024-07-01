using SharpMath.Geometry.Shapes;
using SharpMath.Primitives;

namespace SharpMath.Geometry.Splitting;

public class UniformSplitter : ICurveSplitter
{
    private readonly Positive<int> _steps;

    public UniformSplitter(Positive<int> steps)
    {
        _steps = steps;
    }

    public IEnumerable<TPoint> EnumerateValues<TPoint>(ICurve<TPoint> curve)
    {
        var step = 1d / _steps;

        var stepNumber = 0;
        var t = 0d;

        do
        {
            yield return curve.GetByParameter(t);
            t = stepNumber * step;
            stepNumber++;
        } while (stepNumber < _steps);

        yield return curve.GetByParameter(1d);
    }
}