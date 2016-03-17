using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Exceptions.EmptyCatchClause;

namespace VSDiagnostics.Test.Tests.Exceptions
{
    [TestClass]
    public class EmptyCatchClauseTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new EmptyCatchClauseAnalyzer();

        [TestMethod]
        public void EmptyCatchClause_WithSingleEmptyCatchBlock()
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

            VerifyDiagnostic(original, EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void EmptyCatchClause_WithOnlyTriviaInCatchBlock()
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

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EmptyCatchClause_WithMixOfEmptyAndNonEmptyCatchBlocks()
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
                System.Console.WriteLine(e.Message);
            }
        }
    }
}";

            VerifyDiagnostic(original,
                EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString(),
                EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString(),
                EmptyCatchClauseAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void EmptyCatchClause_WithNonEmptyCatchBlock()
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
                System.Console.WriteLine(e.Message);
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void EmptyCatchClause_WithOnlyCommentsInCatchBlock()
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
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}