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
        public void ConditionIsAlwaysTrue_WithElseIf_ChildIfIsAlwaysTrue_IfIsInBlockNode()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var potentiallyTrue = true;
            if (potentiallyTrue)
            {
                var b = true;
                b = false;
            }
            else
            {
                if (true)
                {
                    var b = true;
                    b = false;
                }
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
            var potentiallyTrue = true;
            if (potentiallyTrue)
            {
                var b = true;
                b = false;
            }
            else
            {
                var b = true;
                b = false;
            }
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_WithElseIf_ChildIfIsAlwaysTrue_IfIsInElseNode()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var potentiallyTrue = true;
            if (potentiallyTrue)
            {
                var b = true;
                b = false;
            }
            else if (true)
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
            var potentiallyTrue = true;
            if (potentiallyTrue)
            {
                var b = true;
                b = false;
            }
            else
            {
                var b = true;
                b = false;
            }
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

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConstantBooleanComparedToBooleanLiteralTrue()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const bool lit = true;
            if (lit == true)
            {
                var dummyVal = 0;
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
            const bool lit = true;
            var dummyVal = 0;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConstantBooleanComparedToBooleanLiteralFalse()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const bool lit = false;
            if (lit == false)
            {
                var dummyVal = 0;
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
            const bool lit = false;
            var dummyVal = 0;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConstantBoolean()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const bool lit = true;
            if (lit)
            {
                var dummyVal = 0;
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
            const bool lit = true;
            var dummyVal = 0;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConstantInteger()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const int lit = 5;
            if (lit == 5)
            {
                var dummyVal = 0;
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
            const int lit = 5;
            var dummyVal = 0;
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysTrueAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_ConstantInteger_DoesNotEvaluateToTrue()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const int lit = 5;
            if (lit == 6) {}
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}