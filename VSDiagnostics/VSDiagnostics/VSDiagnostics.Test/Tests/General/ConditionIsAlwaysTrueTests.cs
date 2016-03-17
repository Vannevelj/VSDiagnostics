using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionIsAlwaysTrue;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionIsAlwaysTrueTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionIsAlwaysTrueAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionIsAlwaysTrueCodeFix();

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (true)
            {
                var b = true;
                b = false;
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;
            b = false;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (true)
                b = false;
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;
            b = false;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConditionContainsTrueLiteral()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (b && true)
                b = false;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_WithElse_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (true)
            {
                var b = true;
                b = false;
            }
            else
            {
                var b = true;

                if (b) { } // prevent variable is never used warning for fix
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;
            b = false;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_WithElse_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (true)
                b = false;
            else
                b = false;

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;
            b = false;

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_WithElseIf_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (true)
            {
                var b = true;
                b = false;
            }
            else if (9 == 8)
            {
                var b = true;
                b = false;
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;
            b = false;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_WithElseIf_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (true)
                b = false;
            else if (9 == 8)
                b = false;

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;
            b = false;

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_WithElseIfElse_ConditionHasBraces()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (true)
            {
                Console.WriteLine("""");
            }
            else if (9 == 8)
            {
                Console.WriteLine("""");
            }
            else
            {
                Console.WriteLine("""");
            }
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Console.WriteLine("""");
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_WithElseIfElse_ConditionDoesNotHaveBraces()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (true)
                Console.WriteLine("""");
            else if (9 == 8)
                b = false;
            else
                Console.WriteLine("""");
        }
    }
}";

            var result = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;
            Console.WriteLine("""");
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}