using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Materials.Providers;

public class DefaultMaterialProvider<TMaterial> : IMaterialProvider<TMaterial> 
    where TMaterial : new()
{
    private static readonly TMaterial Material = new();

    public TMaterial GetById(int materialId) => Material;
}