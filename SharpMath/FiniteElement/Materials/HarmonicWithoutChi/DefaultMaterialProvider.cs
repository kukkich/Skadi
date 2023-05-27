using SharpMath.FiniteElement.Core.Assembling.Params;

namespace SharpMath.FiniteElement.Materials.HarmonicWithoutChi;

public class DefaultMaterialProvider : IMaterialProvider<Material>
{
    private static readonly Material Material = new (1d, 1d, 1d);

    public Material GetById(int materialId) => Material;
}