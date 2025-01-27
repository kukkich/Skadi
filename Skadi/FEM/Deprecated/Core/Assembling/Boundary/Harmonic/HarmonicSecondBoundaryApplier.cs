using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Deprecated.Core.Harmonic_OLD;
using Skadi.Geometry._2D;
using Skadi.Matrices;
using Skadi.Matrices.Sparse;

namespace Skadi.FEM.Deprecated.Core.Assembling.Boundary.Harmonic;

public class HarmonicSecondBoundaryApplier : IHarmonicSecondBoundaryApplier<SparseMatrix>
{
    private readonly Context<Vector2D, IElement, SparseMatrix> _context;
    private readonly IStackInserter<SparseMatrix> _inserter;

    public HarmonicSecondBoundaryApplier(Context<Vector2D, IElement, SparseMatrix> context, IStackInserter<SparseMatrix> inserter)
    {
        _context = context;
        _inserter = inserter;
    }

    public void Apply(Equation<SparseMatrix> equation, HarmonicSecondCondition condition)
    {
        var element = _context.Grid.Elements[condition.ElementId];

        var indexes = GetBoundNodeIndexes(element, condition.LocalBound, stackalloc int[2]);
        for (var i = 0; i < indexes.Length; i++)
        {
            indexes[i] *= 2;
            if (condition.Type is ComponentType.Imaginary)
                indexes[i] += 1;
        }

        var defaultMass = new MatrixSpan([
            2, 1,
            1, 2
        ], 2);

        var (width, lenght) = GetSizes( element);
        
        var massCoef = condition.LocalBound switch
        {
            BoundTypes2D.Bottom or BoundTypes2D.Top => width / 6d,
            BoundTypes2D.Left or BoundTypes2D.Right => lenght / 6d,
            _ => throw new ArgumentOutOfRangeException()
        };
        LinAl.Multiply(massCoef, defaultMass, defaultMass);

        var conditionImpact = LinAl.Multiply(defaultMass, condition.Thetta, stackalloc double[2]);
        var local = new StackLocalVector(conditionImpact, new StackIndexPermutation(indexes));
        _inserter.InsertVector(equation.RightSide, local);
    }

    private Span<int> GetBoundNodeIndexes(IElement element, int bound, Span<int> memory)
    {
        throw new NotImplementedException();
    }
    
    private (double Width, double Length) GetSizes(IElement element)
    {
        throw new NotImplementedException("Замена для element.Width и element.Length");
    }
}