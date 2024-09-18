using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;
using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Materials.HarmonicWithoutChi;
using SharpMath.Geometry;
using System.Numerics;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Third;

namespace SharpMath.FiniteElement.Core;

public class BoilingContext<TPoint, TElement, TMatrix>
{
    public required Grid<TPoint, TElement> Grid { get; set; }
    public required Equation<TMatrix> Equation { get; set; }
    public required IMaterialProvider<Material> Materials { get; set; }
    public required FirstCondition[] FirstConditions { get; set; }
    public required ThirdCondition[] SecondConditions { get; set; }
}