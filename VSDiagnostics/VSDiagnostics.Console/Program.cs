using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix;
using VSDiagnostics.Diagnostics.Exceptions.ArgumentExceptionWithoutNameofOperator;

namespace VSDiagnostics.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Load solution in memory
            const string solutionFilePath = @"C:\Users\jeroen\Documents\Github\VSDiagnostics\VSDiagnostics\VSDiagnostics.sln";
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionFilePath).Result;

            // Get all analyzers
            var analyzers = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();
            Assembly.GetAssembly(typeof(AsyncMethodWithoutAsyncSuffixAnalyzer))
                    .GetTypes()
                    .Where(x => typeof(DiagnosticAnalyzer).IsAssignableFrom(x))
                    .Select(Activator.CreateInstance)
                    .Cast<DiagnosticAnalyzer>()
                    .ToList()
                    .ForEach(x => analyzers.Add(x));

            System.Console.WriteLine($"Found {analyzers.Count} diagnostics.");
            UnderLine();

            // Apply analyzers
            foreach (var project in solution.Projects)
            {
                System.Console.WriteLine($"Analyzing project {project.Name}");
                UnderLine();

                var compilation = project.GetCompilationAsync().Result;
                foreach (var analyzer in analyzers)
                {
                    var diagnosticResults = compilation.WithAnalyzers(ImmutableArray.Create(analyzer)).GetAnalyzerDiagnosticsAsync().Result;
                    var interestingResults = diagnosticResults.Where(x => x.Severity != DiagnosticSeverity.Hidden).ToArray();
                    if (interestingResults.Any())
                    {
                        System.Console.WriteLine($"\n\nResults for analyzer {analyzer}");
                        UnderLine();
                    }
                    
                    foreach (var diagnostic in interestingResults)
                    {
                        if (diagnostic.Severity != DiagnosticSeverity.Hidden)
                        {
                            System.Console.WriteLine($"Severity: {diagnostic.Severity}\tMessage: {diagnostic.GetMessage()}");
                        }
                    }
                }
            }

            // Display diagnostics
            UnderLine();
            System.Console.WriteLine("End of diagnostics");
            System.Console.Read();
        }

        private static void UnderLine()
        {
            System.Console.WriteLine(new string('-', 80));
            System.Console.WriteLine();
        }
    }
}