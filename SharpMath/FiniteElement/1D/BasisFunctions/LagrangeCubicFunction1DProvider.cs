using SharpMath.FEM.Core;
using SharpMath.FiniteElement.Core.BasisFunctions;

namespace SharpMath.FiniteElement._1D.BasisFunctions;

public class LagrangeCubicFunction1DProvider : IBasisFunctionsProvider<IElement, double>
{
    private readonly Grid<double, IElement> _grid;

    public LagrangeCubicFunction1DProvider(Grid<double, IElement> grid)
    {
        _grid = grid;
    }
    
    public IBasisFunction<double>[] GetFunctions(IElement element)
    {
        var left = _grid.Nodes[element.NodeIds[0]];
        var right = _grid.Nodes[element.NodeIds[1]];
        var length = right - left;
        
        Func<double, double>[] templateFunctions =
        [
            t =>
            {
                var x = Shift(t);
                return -9d / 2 * (x - 1d / 3) * (x - 2d / 3) * (x - 1);
            },
            t =>
            {
                var x = Shift(t);
                return 27d / 2 * x * (x - 2d / 3) * (x - 1);
            },
            t =>
            {
                var x = Shift(t);
                return -27d / 2 * x * (x - 1d / 3) * (x - 1);
            },
            t =>
            {
                var x = Shift(t);
                return 9d / 2 * x * (x - 1d / 3) * (x - 2d / 3);
            },
        ];

        return templateFunctions.Select(f => new ExplicitFunction1D(f))
            .ToArray<IBasisFunction<double>>();

        double Shift(double coordinate) => (coordinate - left) / length;
    }
}