using Microsoft.Extensions.Logging;
using Skadi.EquationsSystem.Preconditions;
using Skadi.Numeric;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Solver;

public class BiCGStabSolver<T> : Method<ConjugateGradientSolverConfig>, ISLAESolver<T> 
    where T : ILinearOperator
{
    private readonly IExtendedPreconditionerFactory<T> _preconditionerFactory;
    private IPreconditioner _preconditioner = null!;
    private IPreconditionerPart _preconditionerPart = null!;
    
    public BiCGStabSolver
    (
        IExtendedPreconditionerFactory<T> preconditionerFactory,
        ConjugateGradientSolverConfig config, 
        ILogger logger
    ) : base(config, logger)
    {
        _preconditionerFactory = preconditionerFactory;
    }

    // notations from https://en.wikipedia.org/wiki/Biconjugate_gradient_stabilized_method
    public Vector Solve(Equation<T> equation)
    {
        (_preconditioner, _preconditionerPart) = _preconditionerFactory.Create(equation.Matrix);

        var b = equation.RightSide;
        var x = equation.Solution;
        var A = equation.Matrix;
        var bNorm = b.Norm;
        var r = LinAl.Subtract(b, A.MultiplyOn(x));
        var rLid = r.Copy();
        var ro = Vector.ScalarProduct(rLid, r);
        var p = r.Copy();

        var h = Vector.Create(x.Length);
        var s = Vector.Create(x.Length);
        var nu = Vector.Create(x.Length);
        var y = Vector.Create(x.Length);
        var z = Vector.Create(x.Length);
        var t = Vector.Create(x.Length);
        var tPreconditioned = Vector.Create(x.Length);
        var sPreconditioned = Vector.Create(x.Length);
        
        for (var i = 1; i < Config.MaxIteration && r.Norm / bNorm >= Config.Precision * Config.Precision; i++)
        {
            y = _preconditioner.MultiplyOn(p, y);
            nu = A.MultiplyOn(y, nu);
            var alpha = ro / Vector.ScalarProduct(rLid, nu);
            h = LinAl.LinearCombination(x, y, 1, alpha, h);
            s = LinAl.LinearCombination(r, nu, 1, -alpha, s);
            
            var discrepancy = LinAl.Subtract(b, A.MultiplyOn(h, z), z).Norm; // could pass any result memory
            if (discrepancy / bNorm <= Config.Precision)
            {
                h.CopyTo(x);
                return x;
            }
            
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