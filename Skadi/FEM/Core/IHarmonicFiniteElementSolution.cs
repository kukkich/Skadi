using Skadi.Vectors;

namespace Skadi.FEM.Core;

public interface IHarmonicFiniteElementSolution<in TPoint>
{
    public IReadonlyVector<double> Weights { get; }
    public double Frequency { get; }
    public double Calculate(TPoint point, double time);
    // Todo derivative as gradient
}