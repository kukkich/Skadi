using System.Diagnostics;
using System.Globalization;
using ComplexSolvers;
using Skadi.EquationsSystem;
using Skadi.EquationsSystem.Smoothing;
using Skadi.EquationsSystem.Solver;
using Skadi.EquationsSystem.Solver.Complex;
using Skadi.LinearAlgebra.Matrices.Sparse;
using Skadi.LinearAlgebra.Vectors;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
var slaeRootPath = "C:\\Users\\Витя\\Desktop\\СЛАУ";
var resultPath = "C:\\Users\\Витя\\Desktop\\СЛАУ\\results";

var slaeIds = new[] {4, 3, 2, 1};
var threadsCounts = new[]
{
    1, 
    // 2, 4, Environment.ProcessorCount
};
Dictionary<string, Func<int, ISmoothing>> smoothings = new()
{
    // ["NoSmoothing"] = _ => new NullSmoothing(),
    ["Smoothing"] = threads => new ResidualSmoothing(threads)
};
Dictionary<string, Func<SolverParameters, IObservableSLAESolver<ComplexMatrix>>> solverFactories = new()
{
    // ["CLOS"] = x => new ComplexLocalOptimalSchemeSolver(x.Config, x.Smoothing, x.Threads), 
    ["CCGS"] = x => new ComplexConjugateGradientSolver(x.Config, x.Smoothing, x.Threads)
};

var testsData =
    from solverFactory in solverFactories
    from slaeId in slaeIds 
    from smoothingFactory in smoothings
    from threadsCount in threadsCounts
    select (slaeId, solverFactory, smoothingFactory, threadsCount);


foreach (var (slaeId, solverFactory, smoothingFactory, threadsCount) in testsData)
{
    var testHeader = $"test {slaeId} {solverFactory.Key} {smoothingFactory.Key} {threadsCount} threads";
    Console.WriteLine($"Started {testHeader}");
    
    var currentSlaePath = $"{slaeRootPath}\\{slaeId}\\";
    
    var di = Binary.ReadFile(func => func.ReadDouble(), currentSlaePath + "di")
        .ToArray();
    var idi = Binary.ReadFile(func => func.ReadInt32(), currentSlaePath + "idi")
        .Select(x => x - 1)
        .ToArray();
    var gg = Binary.ReadFile(func => func.ReadDouble(), currentSlaePath + "gg")
        .ToArray();
    var ijg = Binary.ReadFile(func => func.ReadInt32(), currentSlaePath + "ijg")
        .Select(x => x - 1)
        .ToArray();
    var ig = Binary.ReadFile(func => func.ReadInt32(), currentSlaePath + "ig")
        .Select(x => x - 1)
        .ToArray();
    var jg = Binary.ReadFile(func => func.ReadInt32(), currentSlaePath + "jg")
        .Select(x => x - 1)
        .ToArray();
    var b = Binary.ReadFile(func => func.ReadDouble(), currentSlaePath + "pr")
        .ToArray();

    var matrix = new ComplexMatrix(di, gg, idi, ijg, ig, jg);
    var rightSide = new Vector(b);
    var solution = Vector.Create(rightSide.Count);
    var equation = new Equation<ComplexMatrix>(matrix, solution, rightSide);
    
    using var reader = new StreamReader(currentSlaePath + "kuslau");
    var kuslau = reader
        .ReadToEnd()
        .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
        .Select(x => double.Parse(x, NumberStyles.Float))
        .ToArray();
    var solverConfig = new CommonIterationSLAESolverConfig(kuslau[1], (int)kuslau[2]); //kuslau[0] = size

    var solver = solverFactory.Value(new SolverParameters(solverConfig, smoothingFactory.Value(threadsCount), threadsCount));
    
    List<SLAESolverIteration> iterationsLog = new(25000);
    var progress = new ComplexSolvers.Progress<SLAESolverIteration>(x =>
    {
        if (x.Iteration % 100 == 0)
            Console.WriteLine($"{x.Iteration}: {x.Residual}");
        iterationsLog.Add(x);
    });
    
    var stopwatch = Stopwatch.StartNew();
    _ = solver.Solve(equation, progress);
    stopwatch.Stop();
    
    var durationMs = stopwatch.ElapsedMilliseconds;
    Console.WriteLine($"finished {testHeader} {durationMs}-elapsed ms");

    var currentResultPath = $"{resultPath}\\{slaeId}_{solverFactory.Key}_{smoothingFactory.Key}_{threadsCount}.txt";
    using var writer = new StreamWriter(currentResultPath);
    
    writer.WriteLine(iterationsLog.Count);
    writer.WriteLine(durationMs);
    foreach (var (iterationNumber, residual) in iterationsLog)
    {
        writer.WriteLine($"{iterationNumber} {residual:E15}");
    }
    
    Console.WriteLine($"written at {currentResultPath}");
}

Console.WriteLine("Completed");

public record SolverParameters(CommonIterationSLAESolverConfig Config, ISmoothing Smoothing, int Threads);
