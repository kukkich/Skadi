using SharpMath.FiniteElement;
using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement._2D.BasisFunctions;
using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Vectors;
using System;

namespace SharpMath.Splines;

public class SmoothingSpline2D : ISpline2D<Point>
{
    private readonly IBasisFunctionsProvider<Element, Point> _basisFunctionsProvider;
    private readonly Grid<Point, Element> _grid;
    private readonly Vector _qValues;

    public SmoothingSpline2D(
        IBasisFunctionsProvider<Element, Point> basisFunctionsProvider,
        Grid<Point, Element> grid,
        Vector qValues
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        _qValues = qValues;
    }

    public double Calculate(Point point)
    {
        var element = _grid.Elements.First(e => ElementHas(e, point));

        var basisFunctions = _basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                sum += _qValues[element.NodeIndexes[i] * 4 + j] * basisFunctions[i * 4 + j].Evaluate(point);
            }
        }

        return sum;
    }

    public double CalculateDerivativeByX(Point point)
    {
        var element = _grid.Elements.First(e => ElementHas(e, point));
        var xLeftCoordinate = _grid.Nodes[element.NodeIndexes[0]].X;
        var yLeftCoordinate = _grid.Nodes[element.NodeIndexes[0]].Y;

        var basisFunctions =
            GetBasisFunctions(
                BuildHermiteDerivativeBasisFunctions1D(xLeftCoordinate, element.Width, new Func<double, double>[4]),
                BuildHermiteBasisFunctions1D(yLeftCoordinate, element.Length, new Func<double, double>[4])
            );

        var sum = 0d;

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                sum += _qValues[element.NodeIndexes[i] * 4 + j] * basisFunctions[i * 4 + j].Evaluate(point);
            }
        }

        return sum;
    }

    public double CalculateDerivativeByY(Point point)
    {
        var element = _grid.Elements.First(e => ElementHas(e, point));
        var xLeftCoordinate = _grid.Nodes[element.NodeIndexes[0]].X;
        var yLeftCoordinate = _grid.Nodes[element.NodeIndexes[0]].Y;

        var basisFunctions =
            GetBasisFunctions(
                BuildHermiteBasisFunctions1D(xLeftCoordinate, element.Width, new Func<double, double>[4]),
                BuildHermiteDerivativeBasisFunctions1D(yLeftCoordinate, element.Length, new Func<double, double>[4])
            );

        var sum = 0d;

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                sum += _qValues[element.NodeIndexes[i] * 4 + j] * basisFunctions[i * 4 + j].Evaluate(point);
            }
        }

        return sum;
    }

    private bool ElementHas(Element element, Point node)
    {
        var leftBottom = _grid.Nodes[element.NodeIndexes[0]];
        var rightTop = _grid.Nodes[element.NodeIndexes[^1]];

        return leftBottom.X <= node.X && node.X <= rightTop.X &&
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }

    private static Func<double, double>[] BuildHermiteBasisFunctions1D(double leftCoordinateOfBound, double elementBoundSize, Func<double, double>[] basisFunctions1D)
    {
        basisFunctions1D[0] = coordinate => 1 - 3 * Math.Pow(Shift(coordinate), 2) + 2 * Math.Pow(Shift(coordinate), 3);
        basisFunctions1D[1] = coordinate => elementBoundSize * (Shift(coordinate) - 2 * Math.Pow(Shift(coordinate), 2) + Math.Pow(Shift(coordinate), 3));
        basisFunctions1D[2] = coordinate => 3 * Math.Pow(Shift(coordinate), 2) - 2 * Math.Pow(Shift(coordinate), 3);
        basisFunctions1D[3] = coordinate => elementBoundSize * (-Math.Pow(Shift(coordinate), 2) + Math.Pow(Shift(coordinate), 3));

        return basisFunctions1D;

        double Shift(double coordinate) => (coordinate - leftCoordinateOfBound) / elementBoundSize;
    }

    private static Func<double, double>[] BuildHermiteDerivativeBasisFunctions1D(double leftCoordinateOfBound, double elementBoundSize, Func<double, double>[] basisFunctions1D)
    {
        basisFunctions1D[0] = coordinate => -6 * Shift(coordinate) / elementBoundSize + 6 * Math.Pow(Shift(coordinate), 2) / elementBoundSize;
        basisFunctions1D[1] = coordinate => 1 - 4 * Shift(coordinate) + 3 * Math.Pow(Shift(coordinate), 2);
        basisFunctions1D[2] = coordinate => 6 * Shift(coordinate) / elementBoundSize - 6 * Math.Pow(Shift(coordinate), 2) / elementBoundSize;
        basisFunctions1D[3] = coordinate => -2 * Shift(coordinate) + 3 * Math.Pow(Shift(coordinate), 2);

        return basisFunctions1D;

        double Shift(double coordinate) => (coordinate - leftCoordinateOfBound) / elementBoundSize;
    }

    public IBasisFunction<Point>[] GetBasisFunctions(Func<double, double>[] xBasisFunctions1D, Func<double, double>[] yBasisFunctions1D)
    {
        var basisFunctions2D = new BasisFunction2D[xBasisFunctions1D.Length * yBasisFunctions1D.Length];

        for (var i = 0; i < xBasisFunctions1D.Length; i++)
        {
            for (var j = 0; j < yBasisFunctions1D.Length; j++)
            {
                var basisFunctionIndex = i * 4 + j;
                basisFunctions2D[basisFunctionIndex] = new BasisFunction2D(xBasisFunctions1D[Mu(basisFunctionIndex)], yBasisFunctions1D[Nu(basisFunctionIndex)]);
            }
        }

        return basisFunctions2D;
    }

    private static int Mu(int i)
    {
        return 2 * (i / 4 % 2) + i % 2;
    }

    private static int Nu(int i)
    {
        return 2 * (i / 8) + i / 2 % 2;
    }
}