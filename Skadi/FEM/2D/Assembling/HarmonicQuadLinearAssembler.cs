using System.Numerics;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.BasisFunctions._2D;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.Geometry._1D;
using Skadi.Geometry._2D;
using Skadi.Integration;
using Skadi.Matrices;
using Skadi.Vectors;
using Material = Skadi.FEM.Materials.HarmonicWithoutChi.Material;

// ReSharper disable AccessToModifiedClosure

namespace Skadi.FEM._2D.Assembling;

public class HarmonicQuadLinearAssembler(
    IPointsCollection<Vector2D> nodes,
    IAreaProvider<AreaDefinition> areaProvider,
    IIntegrator2D integrator,
    IMaterialProvider<Material> materialProvider,
    IBasisFunctionsProvider<IElement, Vector2D> basisFunctionsProvider,
    IBasisFunctionsDerivativeProvider2D derivativesProvider,
    INodeDefinedParameter<Complex> density,
    IConstantProvider<double> frequency
    ) : IStackLocalAssembler<IElement>
{
    private const int NodesCount = 4;
    private double Omega => frequency.Get();

    public void AssembleMatrix(IElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes)
    {
        var areaId = element.AreaId;
        var area = areaProvider.GetArea(areaId);
        var material = materialProvider.GetById(area.MaterialId);

        var functions = basisFunctionsProvider.GetFunctions(element);
        var dfDx = derivativesProvider.GetDerivativeByX(element);
        var dfDy = derivativesProvider.GetDerivativeByY(element);
        var jacobian = GetJacobian(element);

        Span<double> x = stackalloc double[NodesCount];
        Span<double> y = stackalloc double[NodesCount];
        for (var i = 0; i < NodesCount; i++)
        {
            var node = nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
            indexes.Permutation[2 * i] = element.NodeIds[i] * 2;
            indexes.Permutation[2 * i + 1] = element.NodeIds[i] * 2 + 1;
        }

        var b1 = x[2] - x[0];
        var b2 = x[1] - x[0];
        var b3 = y[2] - y[0];
        var b4 = y[1] - y[0];
        var b5 = x[0] - x[1] - x[2] + x[3];
        var b6 = y[0] - y[1] - y[2] + y[3];
        var alpha0 = (x[1] - x[0]) * (y[2] - y[0]) - (y[1] - y[0]) * (x[2] - x[0]);

        var block = new MatrixSpan(stackalloc double[4], 2);

        for (var i = 0; i < NodesCount; i++)
        {
            for (var j = 0; j < NodesCount; j++)
            {
                var mass = material.Sigma * Omega * integrator.Calculate(
                    p => functions[i].Evaluate(p) * functions[j].Evaluate(p) * jacobian(p),
                    Line1D.Unit,
                    Line1D.Unit
                );
                var stiffness = material.Lambda * double.Sign(alpha0) * integrator.Calculate(
                    p => 1d / jacobian(p) *
                         (
                             (dfDx[i].Evaluate(p) * (b6 * p.X + b3) - dfDy[i].Evaluate(p) * (b6 * p.Y + b4)) *
                             (dfDx[j].Evaluate(p) * (b6 * p.X + b3) - dfDy[j].Evaluate(p) * (b6 * p.Y + b4))
                             +
                             (dfDy[i].Evaluate(p) * (b5 * p.Y + b2) - dfDx[i].Evaluate(p) * (b5 * p.X + b1)) *
                             (dfDy[j].Evaluate(p) * (b5 * p.Y + b2) - dfDx[j].Evaluate(p) * (b5 * p.X + b1))
                         ),
                    Line1D.Unit,
                    Line1D.Unit
                );
                (block[0, 0], block[1, 1]) = (stiffness, stiffness);
                (block[0, 1], block[1, 0]) = (-1d * mass, mass);

                matrixSpan[i * 2, j * 2] = block[0, 0];
                matrixSpan[i * 2, j * 2 + 1] = block[0, 1];
                matrixSpan[i * 2 + 1, j * 2] = block[1, 0];
                matrixSpan[i * 2 + 1, j * 2 + 1] = block[1, 1];
            }
        }
    }

    public void AssembleRightSide(IElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        vector.Nullify();

        var functions = basisFunctionsProvider.GetFunctions(element);
        var jacobian = GetJacobian(element);

        var mass = new MatrixSpan(stackalloc double[NodesCount * NodesCount], NodesCount);
        Span<double> fS = stackalloc double[NodesCount];
        Span<double> fC = stackalloc double[NodesCount];
        Span<double> bS = stackalloc double[NodesCount];
        Span<double> bC = stackalloc double[NodesCount];
        Span<double> x = stackalloc double[NodesCount];
        Span<double> y = stackalloc double[NodesCount];
        for (var i = 0; i < 4; i++)
        {
            var node = nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
            var f = density.Get(element.NodeIds[i]);
            fS[i] = f.Real;
            fC[i] = f.Imaginary;
            indexes.Permutation[2 * i] = element.NodeIds[i] * 2;
            indexes.Permutation[2 * i + 1] = element.NodeIds[i] * 2 + 1;
        }

        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = i; j < element.NodeIds.Count; j++)
            {
                mass[i, j] = integrator.Calculate(
                    p => functions[i].Evaluate(p) * functions[j].Evaluate(p) * jacobian(p),
                    Line1D.Unit,
                    Line1D.Unit
                );

                mass[j, i] = mass[i, j];
            }
        }

        bS = LinAl.Multiply(mass, fS, bS);
        bC = LinAl.Multiply(mass, fC, bC);
        for (var i = 0; i < NodesCount; i++)
        {
            vector[2 * i] = bS[i];
            vector[2 * i + 1] = bC[i];
        }
    }

    private Func<Vector2D, double> GetJacobian(IElement element)
    {
        Span<double> x = stackalloc double[NodesCount];
        Span<double> y = stackalloc double[NodesCount];
        for (var i = 0; i < NodesCount; i++)
        {
            var node = nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
        }

        var alpha0 = (x[1] - x[0]) * (y[2] - y[0]) - (y[1] - y[0]) * (x[2] - x[0]);
        var alpha1 = (x[1] - x[0]) * (y[3] - y[2]) - (y[1] - y[0]) * (x[3] - x[2]);
        var alpha2 = (x[3] - x[1]) * (y[2] - y[0]) - (y[3] - y[1]) * (x[2] - x[0]);

        return p => alpha0 + alpha1 * p.X + alpha2 * p.Y;
    }
}