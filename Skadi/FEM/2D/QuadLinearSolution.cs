using Skadi.FEM.Core;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Vectors;

namespace Skadi.FEM._2D;

public class QuadLinearSolution : IFiniteElementSolution<Point2D>
{
    public IReadonlyVector<double> Weights { get; }

    private const double Epsilon = 1e-10;
    private readonly IBasisFunctionsProvider<IElement, Point2D> _basisFunctionsProvider;
    private readonly Grid<Point2D, IElement> _grid;

    public QuadLinearSolution(
        IBasisFunctionsProvider<IElement, Point2D> basisFunctionsProvider,
        Grid<Point2D, IElement> grid,
        IReadonlyVector<double> weights)
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        Weights = weights;
    }

    public double Calculate(Point2D point)
    {
        var element = _grid.Elements
            .First(x => ElementHas(x, point));

        Span<double> x = stackalloc double[4];
        Span<double> y = stackalloc double[4];
        for (var i = 0; i < 4; i++)
        {
            var node = _grid.Nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
        }

        var b1 = x[2] - x[0];
        var b2 = x[1] - x[0];
        var b3 = y[2] - y[0];
        var b4 = y[1] - y[0];
        var b5 = x[0] - x[1] - x[2] + x[3];
        var b6 = y[0] - y[1] - y[2] + y[3];
        var alpha1 = (x[1] - x[0]) * (y[3] - y[2]) - (y[1] - y[0]) * (x[3] - x[2]);
        var alpha2 = (x[3] - x[1]) * (y[2] - y[0]) - (y[3] - y[1]) * (x[2] - x[0]);

        var w = b6 * (point.X - x[0]) - b5 * (point.Y - y[0]);
        double ksi;
        double eta;

        if (Math.Abs(alpha1) < Epsilon && Math.Abs(alpha2) < Epsilon)
        {
            ksi = (b3 * (point.X - x[0]) - b1 * (point.Y - y[0])) / (b2 * b3 - b1 * b4);
            eta = (b2 * (point.Y - y[0]) - b4 * (point.X - x[0])) / (b2 * b3 - b1 * b4);
        }
        else if (Math.Abs(alpha1) < Epsilon)
        {
            ksi = (alpha2 * (point.X - x[0]) + b1 * w) / (alpha2 * b2 - b5 * w);
            eta = -1d * w / alpha2;
        }
        else if (Math.Abs(alpha2) < Epsilon)
        {
            ksi = w / alpha1;
            eta = (alpha1 * (point.Y - y[0]) - b4 * w) / (alpha1 * b3 + b6 * w);
        }
        else
        {
            throw new NotImplementedException();
        }

        var pointInTemplate = new Point2D(ksi, eta);
        var functions = _basisFunctionsProvider.GetFunctions(element);
        Span<double> funcValues = stackalloc double[functions.Length];
        for (var i = 0; i < functions.Length; i++)
        {
            funcValues[i] = functions[i].Evaluate(pointInTemplate);
        }

        var result = 0d;
        for (var i = 0; i < functions.Length; i++)
        {
            var weightId = element.NodeIds[i];
            var weight = Weights[weightId];
            result += funcValues[i] * weight;
        }
        
        return result;
    }

    public double Derivative(Point2D point)
    {
        throw new NotImplementedException();
    }

    private bool ElementHas(IElement element, Point2D point)
    {
        var nodes = element.NodeIds
            .Select(nodeId => _grid.Nodes[nodeId])
            .ToArray();
        var leftBottom = nodes[0];
        var rightBottom = nodes[1];
        var leftTop = nodes[2];
        var rightTop = nodes[3];

        return IsPointInTriangle(point, leftBottom, rightBottom, leftTop) ||
               IsPointInTriangle(point, leftTop, rightBottom, rightTop);

        bool IsPointInTriangle(Point2D p, Point2D a, Point2D b, Point2D c)
        {
            // Векторные произведения для всех трёх рёбер треугольника
            var v1 = (b - a).X * (p.Y - a.Y) - (b - a).Y * (p.X - a.X);
            var v2 = (c - b).X * (p.Y - b.Y) - (c - b).Y * (p.X - b.X);
            var v3 = (a - c).X * (p.Y - c.Y) - (a - c).Y * (p.X - c.X);

            // Проверяем, что все знаки одинаковы
            return (v1 >= 0 && v2 >= 0 && v3 >= 0) || (v1 <= 0 && v2 <= 0 && v3 <= 0);
        }
    }
}