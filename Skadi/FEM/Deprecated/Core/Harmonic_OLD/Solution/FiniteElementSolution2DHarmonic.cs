using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.FEM.Deprecated.Core.Harmonic_OLD.Solution;

public class FiniteElementSolution2DHarmonic
{
    private readonly Grid<Vector2D, IElement> _grid;
    private readonly Vector _weights;
    private readonly double _frequency;

    public FiniteElementSolution2DHarmonic(
        Grid<Vector2D, IElement> grid, 
        Vector weights,
        double frequency
    )
    {
        _grid = grid;
        _weights = weights;
        _frequency = frequency;
    }

    public double Calculate(Vector2D vector, double time)
    {
        var element = _grid.Elements.First(x => ElementHas(x, vector));

        var leftBottom = _grid.Nodes[element.NodeIds[0]];
        var rightTop = _grid.Nodes[element.NodeIds[^1]];
        var (xMin, xMax) = (leftBottom.X, rightTop.X);
        var (yMin, yMax) = (leftBottom.Y, rightTop.Y);
        var (hx, hy) = (xMax - xMin, yMax - yMin);

        Span<double> x = stackalloc double[2];
        Span<double> y = stackalloc double[2];

        x[0] = (xMax - vector.X) / hx;
        y[0] = (yMax - vector.Y) / hy;
        x[1] = (vector.X - xMin) / hx;
        y[1] = (vector.Y - yMin) / hy;

        Span<double> basicValues = stackalloc double[x.Length * y.Length];

        basicValues[0] = x[0] * y[0];
        basicValues[1] = x[1] * y[0];
        basicValues[2] = x[0] * y[1];
        basicValues[3] = x[1] * y[1];

        var (us, uc) = (0d, 0d);

        for (var i = 0; i < 4; i++)
        {
            var nodeIndex = element.NodeIds[i];
            us += _weights[nodeIndex * 2] * basicValues[i];
            uc += _weights[nodeIndex * 2 + 1] * basicValues[i];
        }

        return us * Math.Sin(_frequency * time)
             + uc * Math.Cos(_frequency * time);
    }

    private bool ElementHas(IElement element, Vector2D node)
    {
        var leftBottom = _grid.Nodes[element.NodeIds[0]];
        var rightTop = _grid.Nodes[element.NodeIds[^1]];

        return leftBottom.X <= node.X && node.X <= rightTop.X
            && leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}