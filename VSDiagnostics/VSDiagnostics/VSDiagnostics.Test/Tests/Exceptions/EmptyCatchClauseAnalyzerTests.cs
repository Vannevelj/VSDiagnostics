using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.EmptyCatchClause;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class EmptyCatchClauseAnalyzerTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new EmptyCatchClauseAnalyzer();

        [TestMethod]
        public void EmptyCatchClauseAnalyzer_WithSingleEmptyCatchBlock_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            try
            {
            }
            catch (Exception)
            {
            }
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = EmptyCatchClauseAnalyzer.Rule.Id,
                Message = EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString(),
                Severity = EmptyCatchClauseAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 13)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void EmptyCatchClauseAnalyzer_WithOnlyTriviaInCatchBlock_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            try
            {
            }
            catch (Exception)
            {
                // just some comments
#region regionInCatch
                // more comments in region
#endregion
            }
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = EmptyCatchClauseAnalyzer.Rule.Id,
                Message = EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString(),
                Severity = EmptyCatchClauseAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 13)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void EmptyCatchClauseAnalyzer_WithMixOfEmptyAndNonEmptyCatchBlocks_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            try
            {
            }
            catch (InvalidCastException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}";
            var expectedDiagnosticInvalidCastException = new DiagnosticResult
            {
                Id = EmptyCatchClauseAnalyzer.Rule.Id,
                Message = EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString(),
                Severity = EmptyCatchClauseAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 13)
                    }
            };
            var expectedDiagnosticArgumentNullException = new DiagnosticResult
            {
                Id = EmptyCatchClauseAnalyzer.Rule.Id,
                Message = EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString(),
                Severity = EmptyCatchClauseAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 17, 13)
                    }
            };
            var expectedDiagnosticArgumentException = new DiagnosticResult
            {
                Id = EmptyCatchClauseAnalyzer.Rule.Id,
                Message = EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString(),
                Severity = EmptyCatchClauseAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 20, 13)
                    }
            };

            VerifyDiagnostic(original,
                expectedDiagnosticInvalidCastException,
                expectedDiagnosticArgumentNullException,
                expectedDiagnosticArgumentException);
        }

        [TestMethod]
        public void EmptyCatchClauseAnalyzer_WithNonEmptyCatchBlock_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            try
            {
            }
            catch (ArgumentNullException e) when (e.ParamName == ""test"")
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }
    }
}