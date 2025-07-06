using Skadi.Geometry.Shapes;

namespace Skadi.Geometry.Splitting;

public readonly record struct UniformSplitter(int Steps) : ICurveSplitter
{
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