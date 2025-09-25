using Skadi.LinearAlgebra.Vectors;

namespace Skadi.FEM.Core;

// ReSharper disable once InconsistentNaming
public interface IVectorFEMSolution<T>
{
    public IReadonlyVector<double> Weights { get; }
    public T Calculate(T point); 
}