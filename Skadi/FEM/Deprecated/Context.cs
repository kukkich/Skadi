using System.Numerics;
using Skadi.FEM.Core.Assembling.Boundary.First;
using Skadi.FEM.Core.Assembling.Params;
using Skadi.FEM.Core.Geometry;
using Skadi.FEM.Deprecated.Core.Assembling.Boundary.Harmonic;
using Skadi.FEM.Materials.HarmonicWithoutChi;

namespace Skadi.FEM.Deprecated;

[Obsolete]
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