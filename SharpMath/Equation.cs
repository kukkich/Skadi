using SharpMath.Vectors;

namespace SharpMath;

//TODO сделать struct
public record Equation<TMatrix>(TMatrix Matrix, Vector Solution, Vector RightSide);