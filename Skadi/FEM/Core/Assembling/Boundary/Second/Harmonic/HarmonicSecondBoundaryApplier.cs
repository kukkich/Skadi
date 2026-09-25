using Skadi.EquationsSystem;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Geometry._2D.Shapes;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM.Core.Assembling.Boundary.Second.Harmonic;

public class HarmonicSecondBoundaryApplier<TMatrix>(
    IPointsCollection<Vector2D> nodes,
    IInserter<TMatrix> inserter
) : IHarmonicSecondBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, HarmonicSecondBoundary condition)
    {
        var edge = condition.Edge;
        var edgeLength = Line2D.GetLength(nodes[condition.Edge.Begin], nodes[condition.Edge.End]);
        
        Span<double> real = stackalloc double[2];
        Span<double> imaginary = stackalloc double[2];
        for (var i = 0; i < real.Length; i++)
        {
            var thetta = condition.Thetta[i];
            (real[i], imaginary[i]) = (thetta.Real, thetta.Imaginary);
        }
        var defaultMass = new MatrixSpan([
            2, 1,
            1, 2
        ], 2);
        var massCoef = edgeLength / 6d;
        MatrixOps.Scale(massCoef, defaultMass, defaultMass);

        Span<double> realImpact = stackalloc double[2];
        Span<double> imaginaryImpact = stackalloc double[2];
        MatrixOps.Multiply(defaultMass, real, realImpact);
        MatrixOps.Multiply(defaultMass, imaginary, imaginaryImpact);
        
        var realLocalVector = new StackLocalVector
        (
            realImpact,
            new StackIndexPermutation([edge.Begin * 2, edge.End * 2])
        );
        var imaginaryLocalVector = new StackLocalVector
        (
            imaginaryImpact, 
            new StackIndexPermutation([edge.Begin * 2 + 1, edge.End * 2 + 1])
        );
        
        inserter.InsertVector(equation.RightSide, realLocalVector);
        inserter.InsertVector(equation.RightSide, imaginaryLocalVector);
    }
}