using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.NullableToShorthand;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NullableToShorthandAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new NullableToShorthandAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new NullableToShorthandCodeFix();

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableLocal_InvokesWarning()
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
            Nullable<float> myVar = 5.0f;
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
            float? myVar = 5.0f;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableField_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {
            private Nullable<int> myVar = 5;
            void Method()
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
            private int? myVar = 5;
            void Method()
            {
                
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 21)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableParameter_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(Nullable<double> myVar)
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
            void Method(double? myVar)
            {
                
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 25)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableTypeParameter_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass<T>
        {   
            void Method()
            {
                var myVar = new MyClass<Nullable<int>>();
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass<T>
        {   
            void Method()
            {
                var myVar = new MyClass<int?>();
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 41)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableProperty_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        public Nullable<int> myVar { get; set; }
        void Method()
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
        public int? myVar { get; set; }
        void Method()
        {
            
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 16)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNonNullableNestedTypeParameter_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass<T>
        {   
            void Method()
            {
                var myVar = new MyClass<Dictionary<MyOtherClass, string>>();
            }
        }

        class MyOtherClass
        {

        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithShorthandNotation_DoesNotInvokeWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            public int? myVar { get; set; }
            void Method()
            {
                
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithChainedShorthandNotation_DoesNotInvokeWarning()
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
                int? myVar, mySecondVar = 5;
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithChainedNullableLocal_InvokesWarning()
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
            Nullable<int> myVar, mySecondVar = 5;
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
            int? myVar, mySecondVar = 5;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 13)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableDefaultParameter_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method(Nullable<int> myVar = 5)
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
            void Method(int? myVar = 5)
            {
                
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 25)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithMultipleNullablesAsGenericParameters_InvokesWarning()
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
                Dictionary<Nullable<int>, Nullable<int>> myVar = null;
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
                Dictionary<int?, int?> myVar = null;
            }
        }
    }";

            var expectedDiagnostic1 = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 28)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 43)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic1, expectedDiagnostic2);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithUnassignedNullableAsLocalVariable_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass<T>
        {   
            void Method()
            {
                new MyClass<Nullable<int>>();
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;

    namespace ConsoleApplication1
    {
        class MyClass<T>
        {   
            void Method()
            {
                new MyClass<int?>();
            }
        }
    }";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "Unnamed variable"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 29)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableTypeAsTypeParameterForNestedDictionaries_InvokesWarning()
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
            Dictionary<Dictionary<int, Nullable<int>>, int> myVar = null;
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
            Dictionary<Dictionary<int, int?>, int> myVar = null;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 40)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthandAnalyzer_WithNullableTypeAsReturnType_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        Nullable<int> Method()
        {
            return null;
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
        int? Method()
        {
            return null;
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.Rule.Id,
                Message = string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "Return statement"),
                Severity = NullableToShorthandAnalyzer.Rule.DefaultSeverity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 9)
                    }
            };

            VerifyDiagnostic(original, expectedDiagnostic);
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }
    }
}