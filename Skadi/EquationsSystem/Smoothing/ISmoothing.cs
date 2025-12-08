using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Smoothing;

public interface ISmoothing
{
    Vector Solution { get; }
    Vector Residual { get; }

    void Initialize(Vector startSolution, Vector startResidual);
    void Apply(Vector currentSolution, Vector currentResidual);
}