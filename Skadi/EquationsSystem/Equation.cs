using Skadi.Vectors;

namespace Skadi;

// TODO replace with IVector<double> Solution and RightSide
public record Equation<TMatrix>(TMatrix Matrix, Vector Solution, Vector RightSide);