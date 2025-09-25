using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Geometry._2D.Shapes;
using Skadi.Matrices;

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
        
        var defaultMass = new MatrixSpan([
            2, 1,
            1, 2
        ], 2);
        var massCoef = edgeLength / 6d;
        LinAl.Multiply(massCoef, defaultMass, defaultMass);

        var realImpact = LinAl.Multiply(defaultMass, real, stackalloc double[2]);
        var imaginaryImpact = LinAl.Multiply(defaultMass, imaginary, stackalloc double[2]);
        
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