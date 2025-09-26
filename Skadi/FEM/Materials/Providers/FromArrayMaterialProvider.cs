using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Materials.Providers;

public class FromArrayMaterialProvider<TMaterial>(TMaterial[] materials) : IMaterialProvider<TMaterial>
{
    public TMaterial GetById(int materialId)
    {
        return materials[materialId];
    }
}