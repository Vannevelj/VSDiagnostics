using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class TryCastUsingAsNotNullInsteadOfIsAsTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new TryCastWithoutUsingAsNotNullAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new TryCastWithoutUsingAsNotNullCodeFix();

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithIsAs_AndReferenceType()
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
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            string oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithIsAs_AndValueType()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt = o as int?;
            if (oAsInt != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithIsAs_AndObjectIsUsedBeforeIs()
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
            object o = ""sample"";
            Console.Write(o.GetType());
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            Console.Write(o.GetType());
            string oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithIsAs_AndObjectIsMethodParameter()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            string oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithIsAs_AndElseClause()
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
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            string oAsString = o as string;
            if (oAsString != null)
            {
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithMultipleCasts()
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
            object o = 5;
            if (o is int)
            {
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt = o as int?;
            if (oAsInt != null)
            {
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithDirectCast()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = (int) o;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithoutCorrespondingCast()
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
            object o = 5;
            object irrelevant = 10.0;
            if (o is int)
            {
                var irrelevantAsDouble = irrelevant as double?;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithChainedVariableDeclaration()
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
            object o = 5;
            if (o is int)
            {
                int? oAsInt = o as int?, x = 10;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            int? oAsInt = o as int?;
            if (oAsInt != null)
            {
                int? x = 10;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }
    }
}