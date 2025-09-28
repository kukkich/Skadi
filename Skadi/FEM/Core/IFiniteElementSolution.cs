using Skadi.LinearAlgebra.Vectors;

namespace Skadi.FEM.Core;

public interface IFiniteElementSolution<TPoint>
{
    public IReadonlyVector<double> Weights { get; }
    public double Calculate(TPoint point); 
    public TPoint Gradient(TPoint point);
}