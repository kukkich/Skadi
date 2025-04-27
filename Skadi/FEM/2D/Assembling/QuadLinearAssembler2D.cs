using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.BasisFunctions._2D;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Materials.LambdaGamma;
using Skadi.Geometry._1D;
using Skadi.Geometry._2D;
using Skadi.Integration;
using Skadi.Matrices;
using Skadi.Vectors;

// ReSharper disable AccessToModifiedClosure

namespace Skadi.FEM._2D.Assembling;

public class QuadLinearAssembler2D : IStackLocalAssembler<IElement>
{
    private readonly IPointsCollection<Vector2D> _nodes;
    private readonly IAreaProvider<AreaDefinition> _areaProvider;
    private readonly IIntegrator2D _integrator;
    private readonly IMaterialProvider<Material> _materialProvider;
    private readonly IBasisFunctionsProvider<IElement, Vector2D> _basisFunctionsProvider;
    private readonly IBasisFunctionsDerivativeProvider2D _derivativesProvider;
    private readonly INodeDefinedParameter<double> _density;

    public QuadLinearAssembler2D(
        IPointsCollection<Vector2D> nodes,
        IAreaProvider<AreaDefinition> areaProvider,
        IIntegrator2D integrator,
        IMaterialProvider<Material> materialProvider,
        IBasisFunctionsProvider<IElement, Vector2D> basisFunctionsProvider,
        IBasisFunctionsDerivativeProvider2D derivativesProvider,
        INodeDefinedParameter<double> density
    )
    {
        _nodes = nodes;
        _areaProvider = areaProvider;
        _integrator = integrator;
        _materialProvider = materialProvider;
        _basisFunctionsProvider = basisFunctionsProvider;
        _derivativesProvider = derivativesProvider;
        _density = density;
    }

    public void AssembleMatrix(IElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes)
    {
        var areaId = element.AreaId;
        var area = _areaProvider.GetArea(areaId);
        var material = _materialProvider.GetById(area.MaterialId);
        
        var functions = _basisFunctionsProvider.GetFunctions(element);
        var dfDx = _derivativesProvider.GetDerivativeByX(element);
        var dfDy = _derivativesProvider.GetDerivativeByY(element);
        var jacobian = GetJacobian(element);

        Span<double> x = stackalloc double[4];
        Span<double> y = stackalloc double[4];
        for (var i = 0; i < 4; i++)
        {
            var node = _nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
            indexes.Permutation[i] = element.NodeIds[i];
        }

        var b1 = x[2] - x[0];
        var b2 = x[1] - x[0];
        var b3 = y[2] - y[0];
        var b4 = y[1] - y[0];
        var b5 = x[0] - x[1] - x[2] + x[3];
        var b6 = y[0] - y[1] - y[2] + y[3];
        var alpha0 = (x[1] - x[0]) * (y[2] - y[0]) - (y[1] - y[0]) * (x[2] - x[0]);

        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = i; j < element.NodeIds.Count; j++)
            {
                var mass = material.Gamma * _integrator.Calculate(
                    p => functions[i].Evaluate(p) * functions[j].Evaluate(p) * jacobian(p),
                    Line1D.Unit,
                    Line1D.Unit
                );
                var stiffness = material.Lambda * double.Sign(alpha0) * _integrator.Calculate(
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
                
                matrixSpan[i, j] = mass + stiffness;
                matrixSpan[j, i] = matrixSpan[i, j];
            }
        }
    }

    public void AssembleRightSide(IElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        vector.Nullify();
        
        var functions = _basisFunctionsProvider.GetFunctions(element);
        var jacobian = GetJacobian(element);
        
        var mass = new MatrixSpan(stackalloc double[vector.Length * vector.Length], vector.Length);
        Span<double> f = stackalloc double[4];
        Span<double> x = stackalloc double[4];
        Span<double> y = stackalloc double[4];
        for (var i = 0; i < 4; i++)
        {
            var node = _nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
            f[i] = _density.Get(element.NodeIds[i]);
            indexes.Permutation[i] = element.NodeIds[i];
        }

        for (var i = 0; i < element.NodeIds.Count; i++)
        {
            for (var j = i; j < element.NodeIds.Count; j++)
            {
                mass[i, j] = _integrator.Calculate(
                    p => functions[i].Evaluate(p) * functions[j].Evaluate(p) * jacobian(p),
                    Line1D.Unit,
                    Line1D.Unit
                );
                
                mass[j, i] = mass[i, j];
            }
        }

        LinAl.Multiply(mass, f, vector);
    }

    private Func<Vector2D, double> GetJacobian(IElement element)
    {
        Span<double> x = stackalloc double[4];
        Span<double> y = stackalloc double[4];
        for (var i = 0; i < 4; i++)
        {
            var node = _nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
        }

        var alpha0 = (x[1] - x[0]) * (y[2] - y[0]) - (y[1] - y[0]) * (x[2] - x[0]);
        var alpha1 = (x[1] - x[0]) * (y[3] - y[2]) - (y[1] - y[0]) * (x[3] - x[2]);
        var alpha2 = (x[3] - x[1]) * (y[2] - y[0]) - (y[3] - y[1]) * (x[2] - x[0]);

        return p => alpha0 + alpha1 * p.X + alpha2 * p.Y;
    }
}