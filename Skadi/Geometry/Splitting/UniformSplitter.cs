using Skadi.Geometry.Shapes;

namespace Skadi.Geometry.Splitting;

public class UniformSplitter : ICurveSplitter
{
    public int Steps { get; }

    public UniformSplitter(int steps)
    {
        Steps = steps;
    }

    public IEnumerable<TPoint> EnumerateValues<TPoint>(IParametricCurve<TPoint> parametricCurve)
    {
        var step = 1d / Steps;

        var stepNumber = 0;
        var t = 0d;

        do
        {
            yield return parametricCurve.GetByParameter(t);
            stepNumber++;
            t = stepNumber * step;
        } while (stepNumber < Steps);

        yield return parametricCurve.GetByParameter(1d);
    }
}