using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement._2D;

public class EquationAssembler
{
    public Equation<SparseMatrix> FinalEquation => _context.Equation;

    private readonly Context<Point, Element, SparseMatrix> _context;
    private readonly IStackLocalAssembler<Element> _localAssembler;
    private readonly IStackInserter<SparseMatrix> _inserter;
    
    public EquationAssembler(
        Context<Point, Element, SparseMatrix> context,
        IStackLocalAssembler<Element> localAssembler,
        IStackInserter<SparseMatrix> inserter
        )
    {
        _context = context;
        _localAssembler = localAssembler;
        _inserter = inserter;
    }

    public Equation<SparseMatrix> BuildEquation(Context<Point, Element, SparseMatrix> context)
    {
        var equation = context.Equation;

        var matrix = new StackMatrix(stackalloc double[8 * 8], 8);
        Span<double> vector = stackalloc double[8];
        var indexes = new StackIndexPermutation(stackalloc int[8]);

        foreach (var element in context.Grid.Elements)
        {
            _localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            _inserter.InsertMatrix(equation.Matrix, localMatrix);

             _localAssembler.AssembleRightSide(element, vector, indexes);
             var localRightSide = new StackLocalVector(vector, indexes);
             _inserter.InsertVector(equation.RightSide, localRightSide);
        }

        return equation;
    }
}