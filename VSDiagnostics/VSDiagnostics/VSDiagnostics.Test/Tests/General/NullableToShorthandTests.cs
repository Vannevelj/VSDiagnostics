using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.NullableToShorthand;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NullableToShorthandTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new NullableToShorthandAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new NullableToShorthandCodeFix();

        [TestMethod]
        public void NullableToShorthand_WithNullableLocal()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNullableField()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNullableParameter()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNullableTypeParameter()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNullableProperty()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNonNullableNestedTypeParameter()
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
        public void NullableToShorthand_WithShorthandNotation()
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
        public void NullableToShorthand_WithChainedShorthandNotation()
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
        public void NullableToShorthand_WithChainedNullableLocal()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNullableDefaultParameter()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithMultipleNullablesAsGenericParameters()
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

            VerifyDiagnostic(original,
                string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"),
                string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithUnassignedNullableAsLocalVariable()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "Unnamed variable"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNullableTypeAsTypeParameterForNestedDictionaries()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "myVar"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_WithNullableTypeAsReturnType()
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

            VerifyDiagnostic(original, string.Format(NullableToShorthandAnalyzer.Rule.MessageFormat.ToString(), "Return statement"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true);
        }

        [TestMethod]
        public void NullableToShorthand_TypeofNullable()
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
            if (typeof(int) == typeof(Nullable<>) { }
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}