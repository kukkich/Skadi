using Skadi.FiniteElement.Materials.HarmonicWithoutChi;
using Skadi.FiniteElement.Core.Assembling.Params;

namespace Skadi.FiniteElement.Materials.Providers;

public class DefaultMaterialProvider<TMaterial> : IMaterialProvider<TMaterial> 
    where TMaterial : new()
{
    private static readonly TMaterial Material = new();

    public TMaterial GetById(int materialId) => Material;
}