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
    private Equation<T> _equation = null!;
    
    public BiCGStabSolver
    (
        IExtendedPreconditionerFactory<T> preconditionerFactory,
        ConjugateGradientSolverConfig config, 
        ILogger logger
    ) : base(config, logger)
    {
        _preconditionerFactory = preconditionerFactory;
    }

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

        for (var i = 1; i < Config.MaxIteration && r.Norm / bNorm >= Config.Precision * Config.Precision; i++)
        {
            var y = _preconditioner.MultiplyOn(p);
            var nu = A.MultiplyOn(y);
            var alpha = ro / Vector.ScalarProduct(rLid, nu);
            var h = LinAl.LinearCombination(x, y, 1, alpha);
            var s = LinAl.LinearCombination(r, nu, 1, -alpha);
            
            //Check that h is solution and quit if so
            var discrepancy = LinAl.Subtract(b, A.MultiplyOn(h)).Norm;
            if (discrepancy / bNorm <= Config.Precision)
            {
                h.CopyTo(x);
                return x;
            }
            
            var z = _preconditioner.MultiplyOn(s);
            var t = A.MultiplyOn(z);
            var tPreconditioned = _preconditionerPart.MultiplyOn(t);
            var sPreconditioned = _preconditionerPart.MultiplyOn(s);
            var omega = Vector.ScalarProduct(tPreconditioned, sPreconditioned) / Vector.ScalarProduct(tPreconditioned, tPreconditioned);
            x = LinAl.LinearCombination(h, z, 1, omega);
            r = LinAl.LinearCombination(s, t, 1, -omega);
            
            //Check that x is solution and quit if so
            discrepancy = r.Norm;
            if (discrepancy / bNorm <= Config.Precision)
            {
                return x;
            }
            
            var roNext = Vector.ScalarProduct(rLid, r);
            var betta = roNext * alpha / (ro * omega);
            p = LinAl.LinearCombination
            (
                r, LinAl.LinearCombination(p, nu, 1, -omega), 
                1, betta
            );
            ro = roNext;
        }

        return x;
    }
}