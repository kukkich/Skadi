using BenchmarkDotNet.Running;
using Skadi.Integration.GeoGebra;
using SkadiBenchmarks.LinearAlgebra;

// BenchmarkRunner.Run<LinearCombinationBenchmarks>();
// Console.ReadLine();


var grid = GeoGebraSerializer.ParseGeoGebraFile("C:\\Users\\Витя\\Desktop\\geogebra.xml");

Console.WriteLine(grid);
