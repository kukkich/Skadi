﻿using Skadi.FEM.Core.Geometry;
using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM.Core.Assembling;

public interface ILocalAssembler<in TElement>
    where TElement : IElement
{
    public LocalMatrix AssembleMatrix(TElement element);
    public LocalVector AssembleRightSide(TElement element);
}

public interface IStackLocalAssembler<in TElement>
    where TElement : IElement
{
    public void AssembleMatrix(TElement element, MatrixSpan matrixSpan, StackIndexPermutation indexes);
    public void AssembleRightSide(TElement element, Span<double> vector, StackIndexPermutation indexes);
}