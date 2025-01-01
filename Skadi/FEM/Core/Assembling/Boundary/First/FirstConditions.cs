namespace Skadi.FEM.Core.Assembling.Boundary.First;

//public record struct FirstConditions(int[] NodesIndexes, double[] Values);
public record struct FirstCondition(int NodeIndex, double Value);