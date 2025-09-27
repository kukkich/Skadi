using Microsoft.Extensions.Logging;
using Skadi.EquationsSystem.Preconditions;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.EquationsSystem.Solver;

public class BiCGStabSolver<T>
(
    IExtendedPreconditionerFactory<T> preconditionerFactory,
    ConjugateGradientSolverConfig config,
    ILogger logger
) : Method<ConjugateGradientSolverConfig>(config, logger), ISLAESolver<T>
    where T : ILinearOperator
{
    private IPreconditioner _preconditioner = null!;
    private IPreconditionerPart _preconditionerPart = null!;

    // notations from https://en.wikipedia.org/wiki/Biconjugate_gradient_stabilized_method
    public Vector Solve(Equation<T> equation)
    {
        (_preconditioner, _preconditionerPart) = preconditionerFactory.Create(equation.Matrix);

        var b = equation.RightSide;
        var x = equation.Solution;
        var A = equation.Matrix;
        var bNorm = b.Norm;
        var r = LinAl.Subtract(b, A.MultiplyOn(x));
        var rLid = r.Copy();
        var ro = Vector.ScalarProduct(rLid, r);
        var p = r.Copy();

        var h = Vector.Create(x.Count);
        var s = Vector.Create(x.Count);
        var nu = Vector.Create(x.Count);
        var y = Vector.Create(x.Count);
        var z = Vector.Create(x.Count);
        var t = Vector.Create(x.Count);
        var tPreconditioned = Vector.Create(x.Count);
        var sPreconditioned = Vector.Create(x.Count);
        
        for (var i = 1; i < Config.MaxIteration; i++)
        {
            y = _preconditioner.MultiplyOn(p, y);
            nu = A.MultiplyOn(y, nu);
            var alpha = ro / Vector.ScalarProduct(rLid, nu);
            h = LinAl.LinearCombination(x, y, 1, alpha, h);
            
            var discrepancy = LinAl.Subtract(b, A.MultiplyOn(h, z), z).Norm; // could pass any result memory
            if (discrepancy / bNorm <= Config.Precision)
            {
                h.CopyTo(x);
                return x;
            }
            
            s = LinAl.LinearCombination(r, nu, 1, -alpha, s);
            z = _preconditioner.MultiplyOn(s, z);
            t = A.MultiplyOn(z, t);
            tPreconditioned = _preconditionerPart.MultiplyOn(t, tPreconditioned);
            sPreconditioned = _preconditionerPart.MultiplyOn(s, sPreconditioned);
            var omega = Vector.ScalarProduct(tPreconditioned, sPreconditioned) / Vector.ScalarProduct(tPreconditioned, tPreconditioned);
            x = LinAl.LinearCombination(h, z, 1, omega, x);
            r = LinAl.LinearCombination(s, t, 1, -omega, r);
            
            discrepancy = r.Norm;
            if (discrepancy / bNorm <= Config.Precision)
            {
                return x;
            }
            
            var roNext = Vector.ScalarProduct(rLid, r);
            var betta = roNext * alpha / (ro * omega);
            p = LinAl.LinearCombination
            (
                r, LinAl.LinearCombination(p, nu, 1, -omega, p), 
                1, betta,
                p
            );
            
            ro = roNext;
        }

        return x;
    }
}