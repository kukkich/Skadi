using Skadi.Matrices.Converters;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Solver;

public class LUSparseThroughProfileConversion : ISLAESolver<SparseMatrix>
{
    public Vector Solve(Equation<SparseMatrix> equation)
    {
        var profile = MatrixConverter.Convert(equation.Matrix);
        var equationProfile = new Equation<ProfileMatrix>(profile, equation.Solution, equation.RightSide);
        var profileSolver = new LUProfile();
        
        return profileSolver.Solve(equationProfile);
    }
}