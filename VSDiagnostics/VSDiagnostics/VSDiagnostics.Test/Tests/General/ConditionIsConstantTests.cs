using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionIsConstant;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionIsConstantTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionIsConstantAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionIsConstantCodeFix();

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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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
            var variable = true;
            if (true)
            {
                var b = true;
                b = false;
            }
            else if (variable)
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
            var variable = true;
            var b = true;
            b = false;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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
            else if (b)
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
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
            var v = true;
            if (true)
            {
                Console.WriteLine("""");
            }
            else if (v)
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
            var v = true;
            Console.WriteLine("""");
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
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
            else if (b)
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysTrue_CompareIdenticalLiterals()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (5 == 5)
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
            var dummyVal = 0;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "true"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (false)
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
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (false)
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
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConditionContainsTrueLiteral()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (b && false)
                b = false;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElse_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (false)
            {
                var b = true;
                b = false;
            }
            else
            {
                var b = true;
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
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElse_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (false)
                b = false;
            else
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElseIf_ConditionHasBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var v = true;
            if (false)
            {
                var b = true;
                b = false;
            }
            else if (v)
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
            var v = true;
            if (v)
            {
                var b = true;
                b = false;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElseIf_ConditionDoesNotHaveBraces()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var b = true;

            if (false)
                b = false;
            else if (b)
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
            if (b)
                b = false;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElseIfElse_ConditionHasBraces()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var v = true;
            if (false)
            {
                var b = true;
                b = false;
            }
            else if (v)
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
            var v = true;
            if (v)
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

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElseIfElse_ConditionDoesNotHaveBraces()
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

            if (false)
                b = false;
            else if (b)
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
            if (b)
                b = false;
            else
                Console.WriteLine("""");
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConstantBooleanComparedToBooleanLiteralTrue()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const bool lit = false;
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
            const bool lit = false;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConstantBooleanComparedToBooleanLiteralFalse()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const bool lit = true;
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
            const bool lit = true;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConstantBoolean()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const bool lit = false;
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
            const bool lit = false;
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_ConstantInteger()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            const int lit = 5;
            if (lit == 4)
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
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_CompareNonidenticalLiterals()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            if (5 == 4)
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
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS0219");
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElseIf_ChildIfIsAlwaysFalse_IfIsInBlockNode()
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
                if (false)
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
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ConditionIsAlwaysFalse_WithElseIf_ChildIfIsAlwaysFalse_IfIsInElseNode()
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
            else if (false)
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
        }
    }
}";

            VerifyDiagnostic(original, string.Format(ConditionIsConstantAnalyzer.Rule.MessageFormat.ToString(), "false"));
            VerifyFix(original, result);
        }
    }
}