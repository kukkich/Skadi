using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.Geometry._2D;
using Skadi.Matrices;
using Skadi.Splines;

namespace Skadi.FEM._2D.BasisFunctions;

//Наверн надо абстрактный класс ещё сделать, потому что контексты разные могут быть
public class HermiteBasisFunctions2DProvider : IBasisFunctionsProvider<IElement, Point2D>
{
    private readonly SplineContext<Point2D, IElement, Matrix> _context;
    private static readonly IBasisFunction<Point2D>[] BasisFunctions2D;
    private static readonly Func<double, double>[] XBasisFunctions1D;
    private static readonly Func<double, double>[] YBasisFunctions1D;

    static HermiteBasisFunctions2DProvider()
    {
        BasisFunctions2D = new IBasisFunction<Point2D>[16];
        XBasisFunctions1D = new Func<double, double>[4];
        YBasisFunctions1D = new Func<double, double>[4];
    }

    public HermiteBasisFunctions2DProvider(SplineContext<Point2D, IElement, Matrix> context)
    {
        _context = context;
    }

    public IBasisFunction<Point2D>[] GetFunctions(IElement element)
    {
        var firstNodeOfElement = _context.Grid.Nodes[element.NodeIds[0]];
        var (width, lenght) = GetSizes(element);
        var xBasisFunctions1D = BuildHermiteBasisFunctions1D(firstNodeOfElement.X, width, XBasisFunctions1D);
        var yBasisFunctions1D = BuildHermiteBasisFunctions1D(firstNodeOfElement.Y, lenght, YBasisFunctions1D);

        for (var i = 0; i < xBasisFunctions1D.Length; i++)
        {
            for (var j = 0; j < yBasisFunctions1D.Length; j++)
            {
                var basisFunctionIndex = i * 4 + j;
                BasisFunctions2D[basisFunctionIndex] = new BasisFunction2D(xBasisFunctions1D[Mu(basisFunctionIndex)], yBasisFunctions1D[Nu(basisFunctionIndex)]);
            }
        }

        return BasisFunctions2D;
    }

    //Потенциально надо сделать провайдер для одномерных и оттуда их создавать
    private static Func<double, double>[] BuildHermiteBasisFunctions1D(double leftCoordinateOfBound, double elementBoundSize, Func<double, double>[] basisFunctions1D)
    {
        basisFunctions1D[0] = coordinate => 1 - 3 * Math.Pow(Shift(coordinate), 2) + 2 * Math.Pow(Shift(coordinate), 3);
        basisFunctions1D[1] = coordinate => elementBoundSize * (Shift(coordinate) - 2 * Math.Pow(Shift(coordinate), 2) + Math.Pow(Shift(coordinate), 3));
        basisFunctions1D[2] = coordinate => 3 * Math.Pow(Shift(coordinate), 2) - 2 * Math.Pow(Shift(coordinate), 3);
        basisFunctions1D[3] = coordinate => elementBoundSize * (-Math.Pow(Shift(coordinate), 2) + Math.Pow(Shift(coordinate), 3));

        return basisFunctions1D;

        double Shift(double coordinate) => (coordinate - leftCoordinateOfBound) / elementBoundSize;
    }

    private static int Mu(int i)
    {
        return 2 * (i / 4 % 2) + i % 2;
    }

    private static int Nu(int i)
    {
        return 2 * (i / 8) + i / 2 % 2;
    }

    private (double Width, double Length) GetSizes(IElement element)
    {
        throw new NotImplementedException("Замена для element.Width и element.Length");
    }
}