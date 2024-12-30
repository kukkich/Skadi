using Microsoft.Extensions.Configuration;
using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Materials.Providers;

namespace SharpMath.FiniteElement.Materials.LambdaGamma;

public class MaterialReader(IConfiguration configuration)
{
    public IMaterialProvider<Material> Get()
    {
        var path = configuration["Materials"] 
                   ?? throw new Exception("Materials path not found in configuration.");

        using var reader = new StreamReader(path);
        var line = reader.ReadLine();
        List<Material> materials = new();
        
        while (!string.IsNullOrWhiteSpace(line))
        {
            var values = line.Split(' ')
                .Select(double.Parse)
                .ToArray();
            if (values.Length != 2)
            {
                throw new Exception($"Invalid data for line: {line}.");
            }
            materials.Add(new Material(values[0], values[1]));
            line = reader.ReadLine();
        }

        return new FromArrayMaterialProvider<Material>(materials.ToArray());
    }
}