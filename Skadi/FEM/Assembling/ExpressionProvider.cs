﻿using System.Linq.Expressions;
using Skadi.FEM.Core.Assembling.Params;

namespace Skadi.FEM.Assembling;

public class ArrayExpressionProvider: IExpressionProvider
{
    private readonly IReadOnlyList<LambdaExpression> _expressions;

    public ArrayExpressionProvider(IReadOnlyList<LambdaExpression> expressions)
    {
        _expressions = expressions;
    }

    public LambdaExpression GetExpression(int id) => _expressions[id];
}