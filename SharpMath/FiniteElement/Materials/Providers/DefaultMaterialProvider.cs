using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Materials.HarmonicWithoutChi;

namespace SharpMath.FiniteElement.Materials.Providers;

public class DefaultMaterialProvider<TMaterial> : IMaterialProvider<TMaterial> 
    where TMaterial : new()
{
    private static readonly TMaterial Material = new();

    public TMaterial GetById(int materialId) => Material;
}