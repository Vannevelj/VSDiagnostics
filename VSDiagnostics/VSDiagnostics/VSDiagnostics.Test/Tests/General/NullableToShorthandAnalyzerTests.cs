using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using VSDiagnostics.Diagnostics.General.NullableToShorthand;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NullableToShorthandAnalyzerTests : CodeFixVerifier
    {
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 21)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 25)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 41)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 20)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
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
            VerifyCSharpDiagnostic(original);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 17)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 25)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 28)
                    }
            };

            var expectedDiagnostic2 = new DiagnosticResult
            {
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 43)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic1, expectedDiagnostic2);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "Unnamed variable"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 29)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Dictionary<Dictionary<int, Nullable<int>, int> myVar = null;
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "myVar"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 11, 44)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
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
                Id = NullableToShorthandAnalyzer.DiagnosticId,
                Message = string.Format(NullableToShorthandAnalyzer.Message, "Return statement"),
                Severity = NullableToShorthandAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result, allowNewCompilerDiagnostics: true);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new NullableToShorthandCodeFix();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NullableToShorthandAnalyzer();
        }
    }
}