using Skadi.LinearAlgebra.Vectors;

namespace Skadi.FEM.Core.Assembling;

public interface IInserter<in TMatrix>
{
    public void InsertVector(Vector vector, StackLocalVector localVector);
    public void InsertMatrix(TMatrix matrix, StackLocalMatrix localMatrix);
}