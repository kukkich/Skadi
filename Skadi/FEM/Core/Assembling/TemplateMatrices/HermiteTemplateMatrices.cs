﻿using Skadi.LinearAlgebra.Matrices;

namespace Skadi.FEM.Core.Assembling.TemplateMatrices;

public static class HermiteTemplateMatrices
{
    public static ImmutableMatrix HermiteStiffness1D(double elementBoundSize)
    {
        return new ImmutableMatrix
            (
                new[,]
                {
                    {36, 3 * elementBoundSize, -36, 3 * elementBoundSize},
                    {3 * elementBoundSize, 4 * Math.Pow(elementBoundSize, 2), -3 * elementBoundSize, -Math.Pow(elementBoundSize, 2)},
                    {-36, -3 * elementBoundSize, 36, -3 * elementBoundSize},
                    {3 * elementBoundSize, -Math.Pow(elementBoundSize, 2), -3 * elementBoundSize, 4 * Math.Pow(elementBoundSize, 2)},
                }, 
                 1 / (30 * elementBoundSize)
            );
    }

    public static ImmutableMatrix HermiteMass1D(double elementBoundSize)
    {
        return new ImmutableMatrix
            (
                new[,]
                {
                    {156, 22 * elementBoundSize, 54, -13 * elementBoundSize},
                    {22 * elementBoundSize, 4 * Math.Pow(elementBoundSize, 2), 13 * elementBoundSize, -3 * Math.Pow(elementBoundSize, 2)},
                    {54, 13 * elementBoundSize, 156, -22 * elementBoundSize},
                    {-13 * elementBoundSize, -3 * Math.Pow(elementBoundSize, 2), -22 * elementBoundSize, 4 * Math.Pow(elementBoundSize, 2)},
                },
                elementBoundSize / 420
            );
    }
}