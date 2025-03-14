﻿using Microsoft.Extensions.Logging;
using Skadi.Matrices;
using Skadi.Vectors;

namespace Skadi.EquationsSystem.Solver;

public class GaussZeidelSolver : Method<GaussZeidelConfig>, IAllocationRequired<int>
{
    private int Dimension => _currentSolution.Length;

    private Vector _discrepancyVector;
    private Vector _currentSolution;
    private MatrixBase _matrix;
    private IReadonlyVector<double> _rightSide;
    private double _rightSideNorm;

    public GaussZeidelSolver(GaussZeidelConfig config, ILogger<GaussZeidelSolver> logger)
        : base(config, logger)
    {
    }

    public void Allocate(int dimensionSize)
    {
        _discrepancyVector = Vector.Create(dimensionSize);
        _currentSolution = Vector.Create(dimensionSize);
    }

    public Vector Solve(MatrixBase matrix, IReadonlyVector<double> rightSide)
    {
        _matrix = matrix;
        _rightSide = rightSide;
        _rightSideNorm = rightSide.Norm;
        _currentSolution.Nullify();

        var currentPrecision = GetRelativeDiscrepancy(_currentSolution);
        var i = 0;
        for (; i < Config.MaxIteration && currentPrecision > Config.Precision; i++)
        {
            currentPrecision = Iterate();
            Logger.LogDebug("[{iteration}] {relativeDiscrepancy:E8} -- relativeDiscrepancy", i, currentPrecision);
        }

        Logger.LogInformation("GZ ended: [{iteration}] {relativeDiscrepancy:E8} -- relativeDiscrepancy", i,
            currentPrecision);

        return _currentSolution;
    }

    private double Iterate()
    {
        for (var row = 0; row < Dimension; row++)
        {
            var step = 0d;

            for (var j = 0; j < Dimension; j++)
            {
                var a = _matrix[row, j];
                var b = _currentSolution[j];
                var value = a * b;
                step -= value;
            }

            step += _rightSide[row];
            var diagonalValue = _matrix[row, row];
            step *= Config.Relaxation / diagonalValue;

            _currentSolution[row] += step;
        }

        return GetRelativeDiscrepancy(_currentSolution);
    }

    private double GetRelativeDiscrepancy(IReadonlyVector<double> solution)
    {
        _discrepancyVector.Nullify();
        _discrepancyVector = LinAl.Multiply(_matrix, solution, _discrepancyVector);
        _discrepancyVector = LinAl.Subtract(_rightSide, _discrepancyVector);
        return _discrepancyVector.Norm / _rightSideNorm;
    }
}

public class GaussZeidelConfig
{
    public int MaxIteration { get; set; } = 1000;
    public double Precision { get; set; } = 1e-8;
    public double Relaxation { get; set; } = 1;
}