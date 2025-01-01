using Skadi.FiniteElement.Core.Assembling.Params;

namespace Skadi.FiniteElement.Materials.Providers;

public class FromArrayMaterialProvider<TMaterial> : IMaterialProvider<TMaterial>
{
    private readonly TMaterial[] _materials;

    public FromArrayMaterialProvider(TMaterial[] materials)
    {
        _materials = materials;
    }

    public TMaterial GetById(int materialId)
    {
        return _materials[materialId];
    }
}