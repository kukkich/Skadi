using Skadi.Vectors;

namespace Skadi.FiniteElement.Core;

public interface IFiniteElementSolution<in TPoint>
{
    public IReadonlyVector<double> Weights { get; }
    public double Calculate(TPoint point);
    public double Derivative(TPoint point);
}