using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Preconditions;

public interface IPreconditioner
{
    public Vector MultiplyOn(Vector v, Vector? resultMemory = null);
}
