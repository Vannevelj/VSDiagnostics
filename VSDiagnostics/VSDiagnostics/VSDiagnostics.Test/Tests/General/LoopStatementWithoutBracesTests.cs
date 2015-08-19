using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.LoopStatementWithoutBraces;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class LoopStatementWithoutBracesTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new LoopStatementWithoutBracesAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new LoopStatementWithoutBracesCodeFix();

        [TestMethod]
        public void LoopStatementWithoutBraces_For_WithoutBraces_OnSameLine_InvokesWarning()
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
            for (int i = 0; i < 10; i++) Console.WriteLine(""true"");
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
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_For_WithoutBraces_OnSameLine_WithIntermittentComment_InvokesWarning()
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
            for (int i = 0; i < 10; i++) /* comments */ Console.WriteLine(""true"");
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
            for (int i = 0; i < 10; i++) /* comments */
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_For_WithoutBraces_OnNextLine_InvokesWarning()
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
            for (int i = 0; i < 10; i++)
                Console.WriteLine(""true"");
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
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_For_WithBraces_DoesNotInvokeWarning()
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
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_While_WithoutBraces_OnSameLine_InvokesWarning()
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
            while (true) Console.WriteLine(""true"");
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
            while (true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_While_WithoutBraces_OnSameLine_WithIntermittentComment_InvokesWarning()
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
            while (true) /* comments */ Console.WriteLine(""true"");
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
            while (true) /* comments */
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_While_WithoutBraces_OnNextLine_InvokesWarning()
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
            while (true)
                Console.WriteLine(""true"");
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
            while (true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_While_WithBraces_DoesNotInvokeWarning()
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
            while (true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_Foreach_WithoutBraces_OnSameLine_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list) Console.WriteLine(""true"");
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_Foreach_WithoutBraces_OnSameLine_WithIntermittentComment_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list) /* comments */ Console.WriteLine(""true"");
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list) /* comments */
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_Foreach_WithoutBraces_OnNextLine_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
                Console.WriteLine(""true"");
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_Foreach_WithBraces_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_DoWhile_WithoutBraces_OnSameLine_InvokesWarning()
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
            do Console.WriteLine(""true""); while (true);
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
            do
            {
                Console.WriteLine(""true"");
            }
            while (true);
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_DoWhile_WithoutBraces_OnSameLine_WithIntermittentComment_InvokesWarning()
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
            do /* comments */
                Console.WriteLine(""true"");
            while (true);
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
            do /* comments */
            {
                Console.WriteLine(""true"");
            }
            while (true);
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_DoWhile_WithoutBraces_OnNextLine_InvokesWarning()
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
            do
                Console.WriteLine(""true"");
            while (true);
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
            do
            {
                Console.WriteLine(""true"");
            }
            while (true);
        }
    }
}";

            VerifyDiagnostic(original, LoopStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void LoopStatementWithoutBraces_DoWhile_WithBraces_DoesNotInvokeWarning()
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
            do
            {
                Console.WriteLine(""true"");
            } while (true);
        }
    }
}";
            VerifyDiagnostic(original);
        }
    }
}