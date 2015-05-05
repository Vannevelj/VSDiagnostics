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
                Nullable<int> myVar = 5;
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
                int? myVar = 5;
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
            private Nullable<int> _myVar = 5;
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
            private int? _myVar = 5; 
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
            void Method(Nullable<int> myVar)
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
            void Method(int? myVar)
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
                var obj = new MyClass<Nullable<T>>();
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
                var obj = new MyClass<int?>();
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
            void Method(Nullable<int> x = 5)
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
            void Method(int? x = 5)
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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
                Dictionary<Nullable<int>, Nullable<int>> myDic = null;
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
                Dictionary<int?, int?> myDic = null;
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
                        new DiagnosticResultLocation("Test0.cs", 9, 13)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
            VerifyCSharpFix(original, result);
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