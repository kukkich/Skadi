using Skadi.Primitives;
using Skadi.Geometry.Shapes;

namespace Skadi.Geometry.Splitting;

public class UniformSplitter : ICurveSplitter
{
    public Positive<int> Steps { get; }

    public UniformSplitter(Positive<int> steps)
    {
        Steps = steps;
    }

    public IEnumerable<TPoint> EnumerateValues<TPoint>(ICurve<TPoint> curve)
    {
        var step = 1d / Steps;

        var stepNumber = 0;
        var t = 0d;

        do
        {
            yield return curve.GetByParameter(t);
            stepNumber++;
            t = stepNumber * step;
        } while (stepNumber < Steps);

        yield return curve.GetByParameter(1d);
    }
}