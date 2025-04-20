using Skadi.Vectors;

namespace Skadi.EquationsSystem.Preconditions.Null;

public class NullPreconditioner : IPreconditioner, IPreconditionerPart
{
    public static NullPreconditioner Instance { get; } = new();
    
    public Vector MultiplyOn(IReadonlyVector<double> vector, Vector? resultMemory = null)
    {
        LinAl.ValidateOrAllocateIfNull(vector, ref resultMemory!);
        for (var i = 0; i < vector.Length; i++)
        {
            resultMemory[i] = vector[i];
        }

        return resultMemory;
    }
}