using Skadi.LinearAlgebra.Vectors;

namespace Skadi.FEM.Core;

public interface IFiniteElementSolution<in TPoint>
{
    public IReadonlyVector<double> Weights { get; }
    public double Calculate(TPoint point); 
    // Todo derivative as gradient
}