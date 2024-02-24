using SharpMath.Vectors;

namespace SharpMath;

public record Equation<TMatrix>(TMatrix Matrix, Vector Solution, Vector RightSide);