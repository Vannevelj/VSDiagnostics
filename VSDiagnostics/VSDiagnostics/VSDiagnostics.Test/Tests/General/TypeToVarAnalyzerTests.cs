using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.TypeToVar;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class TypeToVarAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void TypeToVarAnalyzer_WithLocalPredefinedType_InvokesWarning()
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
            int x = 0;
        }
    }
}";
            var newSource = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var x = 0;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TypeToVarAnalyzer.DiagnosticId,
                Message = TypeToVarAnalyzer.Message,
                Severity = TypeToVarAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, newSource);
        }

        [TestMethod]
        public void TypeToVarAnalyzer_WithLocalPredefinedTypeInitializedFromMethod_InvokesWarning()
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
            int x = GetInt();
        }

        int GetInt()
        {
            return 0;
        }
    }
}";
            var newSource = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var x = GetInt();
        }

        int GetInt()
        {
            return 0;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TypeToVarAnalyzer.DiagnosticId,
                Message = TypeToVarAnalyzer.Message,
                Severity = TypeToVarAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, newSource);
        }

        [TestMethod]
        public void TypeToVarAnalyzer_WithLocalDefinedWithVar_DoesNotInvokeWarning()
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
            var x = 0;
        }
    }
}";
            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVarAnalyzer_WithLocalAssignedUsingExplicitConversion_InvokesWarning()
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
            int x = (int)0.0;
        }
    }
}";

            var newSource = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var x = (int)0.0;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = TypeToVarAnalyzer.DiagnosticId,
                Message = TypeToVarAnalyzer.Message,
                Severity = TypeToVarAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, newSource);
        }
        
        [TestMethod]
        public void TypeToVarAnalyzer_WithLocalAssignedUsingImplicitConversion_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            IEnumerable<string> x = new List<string>();
        }
    }
}";

            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVarAnalyzer_WithFieldDeclaration_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        int x = 0;
    }
}";

            VerifyCSharpDiagnostic(original);
        }
        
        [TestMethod]
        public void TypeToVarAnalyzer_WithLocalUninitialized_DoesNotInvokeWarning()
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
            int x;
        }
    }
}";

            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void TypeToVarAnalyzer_WithLocalBaseClassTypeInitializedWithDerivedType_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        Base x = new Derived();
    }

    class Base
    {
    }

    class Derived : Base
    {
    }
}";

            VerifyCSharpDiagnostic(original);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TypeToVarCodeFix();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TypeToVarAnalyzer();
        }
    }
}
