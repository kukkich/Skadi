// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Skadi;
using Skadi.Matrices;
using Skadi.Matrices.Sparse;
using Skadi.Vectors;
using SkadiBenchmarks;

// BenchmarkRunner.Run<MatrixVectorMultiplicationBenchmark>();

LinAl.Multiply(new CSRMatrix([1, 2], [1], [0d]), Vector.Create(1));
Console.ReadLine();


