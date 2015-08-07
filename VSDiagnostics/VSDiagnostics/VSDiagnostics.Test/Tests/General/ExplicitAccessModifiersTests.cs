using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ExplicitAccessModifiers;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ExplicitAccessModifiersTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ExplicitAccessModifiersAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ExplicitAccessModifiersCodeFix();

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    internal class MyClass
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), SyntaxKind.InternalKeyword));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_ContainsNonAccessModifier_InvokesWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    static class MyClass
    {
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    static internal class MyClass
    {
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), SyntaxKind.InternalKeyword));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_ContainsAccessModifier_DoesNotInvokeWarning()
        {
            var original = @"
namespace ConsoleApplication1
{
    public class MyClass
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ExplicitAccessModifiers_ClassDeclaration_OnlyChangesAccessModifiers_InvokesWarning()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    class MyClass
    {
        void Method() { }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    [Obsolete]
    internal class MyClass
    {
        void Method() { }
    }
}";

            VerifyDiagnostic(original, string.Format(ExplicitAccessModifiersAnalyzer.Rule.MessageFormat.ToString(), SyntaxKind.InternalKeyword));
            VerifyFix(original, result);
        }
    }
}