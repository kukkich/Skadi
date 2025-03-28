﻿using Skadi.FEM.Core.Geometry;

namespace Skadi.FEM.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TElement, in TPoint> 
    where TElement : IElement
{
    public IBasisFunction<TPoint>[] GetFunctions(TElement element);
}