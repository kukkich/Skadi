using System.Numerics;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Vectors;
using Vector = Skadi.LinearAlgebra.Vectors.Vector;

namespace Skadi.EquationsSystem.Smoothing;

public class ResidualSmoothing(int threadsCount = 1) : ISmoothing
{
    public Vector Solution { get; private set; } = null!;
    public Vector Residual { get; private set; } = null!;

    private Vector _tmp = null!;
    
    public void Initialize(Vector startSolution, Vector startResidual)
    {
        Solution = startSolution.Copy();
        Residual = startResidual.Copy();

        _tmp = Vector.Create(startSolution.Count);
    }

    public void Apply(Vector currentSolution, Vector currentResidual)
    {
        ParallelLinAl.Subtract(currentResidual, Residual, _tmp, threadsCount);

        var n = ParallelLinAl.ComplexScalarProduct(Residual, _tmp, threadsCount).Real;
        var d = ParallelLinAl.ComplexScalarProduct(_tmp, _tmp, threadsCount).Real;
        var t = (-n / d) switch
        {
            < 0 => 0,
            > 1 => 1,
            var v => v
        };
        var tComplex = new Complex(t, 0);
        var ksi = 1 - tComplex;

        _tmp.Nullify();
        Solution = ParallelLinAl.Multiply(Solution, ksi, Solution, threadsCount);
        Solution = ParallelLinAl.Sum
        (
            Solution, 
            ParallelLinAl.Multiply
            (
                currentSolution, 
                t, 
                _tmp, 
                threadsCount
            ),
            Solution,
            threadsCount
        );
        
        _tmp.Nullify();
        Residual = ParallelLinAl.Multiply(Residual, ksi, Residual, threadsCount);
        Residual = ParallelLinAl.Sum
        (
            Residual, 
            ParallelLinAl.Multiply
            (
                currentResidual, 
                t, 
                _tmp,
                threadsCount
            ),
            Residual,
            threadsCount
        );
    }
}