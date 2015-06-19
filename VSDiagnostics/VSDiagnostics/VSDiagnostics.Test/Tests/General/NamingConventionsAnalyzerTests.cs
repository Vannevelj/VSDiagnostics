using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.NamingConventions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NamingConventionsAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new NamingConventionsAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new NamingConventionsCodeFix();

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPrivateField_AndCapitalStart_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int X;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _x;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "X", "_x"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 21)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPrivateField_AndCapitalStartWithUnderscore_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _X;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _x;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "_X", "_x"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 21)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPublicField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProtectedField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 23)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInternalField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 22)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProtectedInternalField_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected internal int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected internal int X;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 32)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProperty_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int x { get; set; }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int X { get; set; }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "property", "x", "X"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithMethod_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void method()
        {
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "method", "method", "Method"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 14)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithClass_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class myClass
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "class", "myClass", "MyClass"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 11)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithoutPrefix_AndLowerFirstLetter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface something
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "interface", "something", "ISomething"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 15)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithPrefix_AndLowerSecondLetter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface Isomething
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "interface", "Isomething", "ISomething"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 15)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithlowerPrefix_AndCapitalSecondLetter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface iSomething
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "interface", "iSomething", "ISomething"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 15)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithoutPrefix_AndCapitalFirstLetter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface Something
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "interface", "Something", "ISomething"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 15)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithUnderscore_InMiddle_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface Cow_milker
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ICowMilker
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "interface", "Cow_milker", "ICowMilker"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 15)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithUnderscore_AtFront_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface _Something
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "interface", "_Something", "ISomething"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 15)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_WithUnderscore_AtBack_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface Something_
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "interface", "Something_", "ISomething"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 15)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithLocalVariable_InvokesWarning()
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
            var MyVar = 5;
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var myVar = 5;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "local", "MyVar", "myVar"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 17)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithParameter_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(string Param)
        {
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(string param)
        {
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "parameter", "Param", "param"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 28)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithMultipleFields_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int X, Y;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _x, _y;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "X", "_x"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 21)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "Y", "_y"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 24)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, expectedDiagnostic2);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPrivateField_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _x;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProtectedField_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected int X;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInternalField_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int X;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPublicField_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int X;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInternalProtectedField_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal protected int X;
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithProperty_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int X { get; set; }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithMethod_FollowingConventions_DoesNotInvokeWarning()
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
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithClass_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithInterface_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    interface ISomething
    {
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithLocalVariable_FollowingConventions_DoesNotInvokeWarning()
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
            string myVar = string.Empty;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithParameter_FollowingConventions_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(string param)
        {
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithVerbatimIdentifier_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int @class;
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithEscapedIdentifier_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        public int \u0061ss;
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithMultipleDifferentTypes_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int x;
        
        void method()
        {
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        internal int X;
        
        void Method()
        {
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "x", "X"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 22)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "method", "method", "Method"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 14)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, expectedDiagnostic2);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithMultipleSimilarTypes_InvokesWarning()
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
            int X, Y;
        }
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            int x, y;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "local", "X", "x"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 17)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "local", "Y", "y"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 20)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic, expectedDiagnostic2);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithExclusivelySpecialCharacters_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        protected int ___;
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithOneLetterPrivateVariable_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int x;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int _x;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "x", "_x"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 21)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithPrivateField_WithoutAccessModifier_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int X;
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int _x;
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "field", "X", "_x"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithStruct_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    struct myStruct
    {
    }
}";

            var result = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    struct MyStruct
    {
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NamingConventionsAnalyzer.Rule.Id,
                Message = string.Format(NamingConventionsAnalyzer.Rule.MessageFormat.ToString(), "struct", "myStruct", "MyStruct"),
                Severity = NamingConventionsAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 7, 12)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void NamingConventionsAnalyzer_WithStruct_FollowingConventions_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    struct MyStruct
    {
    }
}";

            VerifyDiagnostic(original);
        }
    }
}