using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ConditionIsAlwaysFalse;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ConditionIsAlwaysFalseTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ConditionIsAlwaysFalseAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ConditionIsAlwaysFalseCodeFix();

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

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
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

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
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

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
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

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
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
            if (false)
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
            if (9 == 8)
            {
                var b = true;
                b = false;
            }
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
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
            if (9 == 8)
                b = false;

            if (b) { } // prevent variable is never used warning for fix
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
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
            if (false)
            {
                var b = true;
                b = false;
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
            if (9 == 8)
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

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
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
            if (9 == 8)
                b = false;
            else
                Console.WriteLine("""");
        }
    }
}";

            VerifyDiagnostic(original, ConditionIsAlwaysFalseAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}