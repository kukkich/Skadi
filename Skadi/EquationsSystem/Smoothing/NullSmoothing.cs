using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Smoothing;

public class NullSmoothing : ISmoothing
{
    public Vector Solution { get; private set; } = null!;
    public Vector Residual { get; private set; } = null!;

    public void Initialize(Vector startSolution, Vector startResidual)
    {
        Solution = startSolution;
        Residual = startResidual;
    }

    public void Apply(Vector currentSolution, Vector currentResidual) { }
}