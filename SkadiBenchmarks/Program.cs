// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using SkadiBenchmarks;

BenchmarkRunner.Run<MatrixVectorMultiplicationBenchmark>();
Console.ReadLine();