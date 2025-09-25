﻿using Skadi.EquationsSystem;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.Algorithms.Splines;

public abstract class SplineEquationAssembler<TPoint>
{
    protected abstract int LocalMatrixSize { get; }
    protected readonly IPointsCollection<TPoint> Nodes;
    private readonly ISplineStackLocalAssembler<IElement, TPoint> _splineLocalAssembler;
    private readonly IStackLocalAssembler<IElement> _localAssembler;
    private readonly IInserter<Matrix> _inserter;
    
    public SplineEquationAssembler(
        IPointsCollection<TPoint> nodes,
        ISplineStackLocalAssembler<IElement, TPoint> splineLocalAssembler,
        IStackLocalAssembler<IElement> localAssembler,
        IInserter<Matrix> inserter
    )
    {
        Nodes = nodes;
        _splineLocalAssembler = splineLocalAssembler;
        _localAssembler = localAssembler;
        _inserter = inserter;
    }

    public void BuildEquation(
        Equation<Matrix> equation,
        FuncValue<TPoint>[] functionValues,
        IEnumerable<IElement> elements,
        double[]? weights = null)
    {
        var matrix = new MatrixSpan(stackalloc double[LocalMatrixSize * LocalMatrixSize], LocalMatrixSize);
        Span<double> vector = stackalloc double[LocalMatrixSize];
        var indexes = new StackIndexPermutation(stackalloc int[LocalMatrixSize]);

        for (var i = 0; i < functionValues.Length; i++)
        {
            var currentFunctionValue = functionValues[i];
            var currentWeight = WeightFactory(i);
            var element = elements.First(e => ElementHas(e, currentFunctionValue.Point));

            _splineLocalAssembler.AssembleBasisFunctions(element);
            _splineLocalAssembler.AssembleMatrix(element, currentFunctionValue.Point, currentWeight, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            _inserter.InsertMatrix(equation.Matrix, localMatrix);

            _splineLocalAssembler.AssembleRightSide(element, currentFunctionValue, currentWeight,  vector, indexes);
            var localRightSide = new StackLocalVector(vector, indexes);
            _inserter.InsertVector(equation.RightSide, localRightSide);
        }

        foreach (var element in elements)
        {
            _localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            _inserter.InsertMatrix(equation.Matrix, localMatrix);
        }

        return;

        double WeightFactory(int index) => weights == null ? 1d : weights[index];
    }

    protected abstract bool ElementHas(IElement element, TPoint node);
}