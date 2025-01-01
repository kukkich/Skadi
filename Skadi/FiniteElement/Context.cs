using System.Numerics;
using Skadi.FEM.Core;
using Skadi.FiniteElement.Core.Assembling.Boundary.Second;
using Skadi.Geometry;
using Skadi.FiniteElement.Core.Assembling.Boundary.First;
using Skadi.FiniteElement.Core.Assembling.Boundary.Second.Harmonic;
using Skadi.FiniteElement.Core.Assembling.Params;
using Skadi.FiniteElement.Materials.HarmonicWithoutChi;

namespace Skadi.FiniteElement;

public class Context<TPoint, TElement, TMatrix> 
    where TElement : IElement
{
    public required Grid<TPoint, TElement> Grid { get; set; }
    public required Equation<TMatrix> Equation { get; set; }
    public required INodeDefinedParameter<Complex> DensityFunctionProvider { get; set; }
    public required IMaterialProvider<Material> Materials { get; set; }
    public required FirstCondition[] FirstConditions { get; set; }
    public required HarmonicSecondCondition[] SecondConditions { get; set; }
}