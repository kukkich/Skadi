using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Materials.LambdaGamma;
using Skadi.Geometry._1D;
using Skadi.Geometry._2D;
using Skadi.Integration;
using Skadi.Matrices;
using Skadi.Vectors;

namespace Skadi.FEM._2D.Assembling;

public class VectorLinearLocalAssembler : IStackLocalAssembler<IEdgeElement>
{
    private readonly IPointsCollection<Vector2D> _nodes;
    private readonly IAreaProvider<AreaDefinition> _areaProvider;
    private readonly IIntegrator2D _integrator;
    private readonly IMaterialProvider<Material> _materialProvider;
    private readonly IEdgeVectorBasisFunctionsProvider<IEdgeElement, Vector2D> _basisFunctionsProvider;
    private readonly IUniversalParameterProvider<Vector2D, Vector2D> _density;
    
    public VectorLinearLocalAssembler
    (
        IPointsCollection<Vector2D> nodes,
        IAreaProvider<AreaDefinition> areaProvider,
        IIntegrator2D integrator,
        IMaterialProvider<Material> materialProvider,
        IEdgeVectorBasisFunctionsProvider<IEdgeElement, Vector2D> basisFunctionsProvider,
        IUniversalParameterProvider<Vector2D, Vector2D> density
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _density = density;
        _nodes = nodes;
        _areaProvider = areaProvider;
        _integrator = integrator;
        _materialProvider = materialProvider;
    }
    
    public void AssembleMatrix(IEdgeElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes)
    {
        var areaId = element.AreaId;
        var area = _areaProvider.GetArea(areaId);
        var material = _materialProvider.GetById(area.MaterialId);
        var functions = _basisFunctionsProvider.GetFunctions(element);
        
        Span<double> x = stackalloc double[4];
        Span<double> y = stackalloc double[4];
        for (var i = 0; i < 4; i++)
        {
            var node = _nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
            indexes.Permutation[i] = element.EdgeIds[i];
        }

        for (var i = 0; i < element.EdgeIds.Count; i++)
        {
            for (var j = i; j < element.EdgeIds.Count; j++)
            {
                var mass = material.Gamma * _integrator.Calculate(
                    p => functions[i].Evaluate(p).ScalarProduct(functions[j].Evaluate(p)),
                    new Line1D(x[0], x[1]),
                    new Line1D(y[0], y[2])
                );
                var stiffness = material.Lambda * _integrator.Calculate(
                    p => functions[i].Curl(p).Z * functions[j].Curl(p).Z,
                    new Line1D(x[0], x[1]),
                    new Line1D(y[0], y[2])
                );
                
                matrixSpan[i, j] = mass;
                matrixSpan[j, i] = matrixSpan[i, j];
            }
        }
    }

    public void AssembleRightSide(IEdgeElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        vector.Nullify();
        var functions = _basisFunctionsProvider.GetFunctions(element);
        
        var mass = new MatrixSpan(stackalloc double[vector.Length * vector.Length], vector.Length);
        Span<double> f = stackalloc double[4];
        Span<Vector2D> nodes = stackalloc Vector2D[4];
        for (var i = 0; i < 4; i++)
        {
            nodes[i] = _nodes[element.NodeIds[i]];
            indexes.Permutation[i] = element.EdgeIds[i];
        }

        f[0] = _density.Get((nodes[0] + nodes[2]) / 2).Y;
        f[1] = _density.Get((nodes[1] + nodes[3]) / 2).Y;
        f[2] = _density.Get((nodes[0] + nodes[1]) / 2).X;
        f[3] = _density.Get((nodes[2] + nodes[3]) / 2).X;
        
        for (var i = 0; i < element.EdgeIds.Count; i++)
        {
            for (var j = i; j < element.EdgeIds.Count; j++)
            {
                mass[i, j] = _integrator.Calculate(
                    p => functions[i].Evaluate(p).ScalarProduct(functions[j].Evaluate(p)),
                    new Line1D(nodes[0].X, nodes[1].X),
                    new Line1D(nodes[0].Y, nodes[2].Y)
                );
                
                mass[j, i] = mass[i, j];
            }
        }

        LinAl.Multiply(mass, f, vector);
    }
}