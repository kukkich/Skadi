using Skadi.Algorithms.Integration;
using Skadi.FEM.Core.Assembling;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.BasisFunctions;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Core.Geometry._2D.Quad;
using Skadi.FEM.Materials.LambdaGamma;
using Skadi.Geometry._1D;
using Skadi.Geometry._2D;
using Skadi.LinearAlgebra;
using Skadi.LinearAlgebra.Matrices;
using Skadi.LinearAlgebra.Vectors;

namespace Skadi.FEM._2D.Assembling;

public class VectorLinearLocalAssembler
(
    IPointsCollection<Vector2D> nodes,
    IAreaProvider<AreaDefinition> areaProvider,
    IIntegrator2D integrator,
    IMaterialProvider<Material> materialProvider,
    IEdgeVectorBasisFunctionsProvider<IEdgeElement, Vector2D> basisFunctionsProvider,
    IUniversalParameterProvider<Vector2D, Vector2D> density
) : IStackLocalAssembler<IEdgeElement>
{
    public void AssembleMatrix(IEdgeElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes)
    {
        var areaId = element.AreaId;
        var area = areaProvider.GetArea(areaId);
        var material = materialProvider.GetById(area.MaterialId);
        var functions = basisFunctionsProvider.GetFunctions(element);
        
        Span<double> x = stackalloc double[4];
        Span<double> y = stackalloc double[4];
        for (var i = 0; i < 4; i++)
        {
            var node = nodes[element.NodeIds[i]];
            x[i] = node.X;
            y[i] = node.Y;
            indexes.Permutation[i] = element.EdgeIds[i];
        }

        for (var i = 0; i < element.EdgeIds.Count; i++)
        {
            for (var j = i; j < element.EdgeIds.Count; j++)
            {
                var mass = material.Gamma * integrator.Calculate(
                    p => functions[i].Evaluate(p).ScalarProduct(functions[j].Evaluate(p)),
                    new Line1D(x[0], x[1]),
                    new Line1D(y[0], y[2])
                );
                var stiffness = material.Lambda * integrator.Calculate(
                    p => functions[i].Curl(p).Z * functions[j].Curl(p).Z,
                    new Line1D(x[0], x[1]),
                    new Line1D(y[0], y[2])
                );
                
                matrixSpan[i, j] = mass + stiffness;
                matrixSpan[j, i] = matrixSpan[i, j];
            }
        }
    }

    public void AssembleRightSide(IEdgeElement element, Span<double> vector, StackIndexPermutation indexes)
    {
        vector.Nullify();
        var functions = basisFunctionsProvider.GetFunctions(element);
        
        var mass = new MatrixSpan(stackalloc double[vector.Length * vector.Length], vector.Length);
        Span<double> f = stackalloc double[4];
        Span<Vector2D> nodes1 = stackalloc Vector2D[4];
        for (var i = 0; i < 4; i++)
        {
            nodes1[i] = nodes[element.NodeIds[i]];
            indexes.Permutation[i] = element.EdgeIds[i];
        }

        f[0] = density.Get((nodes1[0] + nodes1[2]) / 2).Y;
        f[1] = density.Get((nodes1[1] + nodes1[3]) / 2).Y;
        f[2] = density.Get((nodes1[0] + nodes1[1]) / 2).X;
        f[3] = density.Get((nodes1[2] + nodes1[3]) / 2).X;
        
        for (var i = 0; i < element.EdgeIds.Count; i++)
        {
            for (var j = i; j < element.EdgeIds.Count; j++)
            {
                mass[i, j] = integrator.Calculate(
                    p => functions[i].Evaluate(p).ScalarProduct(functions[j].Evaluate(p)),
                    new Line1D(nodes1[0].X, nodes1[1].X),
                    new Line1D(nodes1[0].Y, nodes1[2].Y)
                );
                
                mass[j, i] = mass[i, j];
            }
        }

        LinAl.Multiply(mass, f, vector);
    }
}