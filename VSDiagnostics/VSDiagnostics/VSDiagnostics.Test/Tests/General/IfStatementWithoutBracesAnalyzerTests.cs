using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.IfStatementWithoutBraces;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class IfStatementWithoutBracesAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new IfStatementWithoutBracesAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new IfStatementWithoutBracesCodeFix();

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_WithoutBraces_OnSameLine_InvokesWarning()
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
            if(true) Console.WriteLine(""true"");
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, IfStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_WithoutBraces_OnSameLine_WithIntermittentComment_InvokesWarning()
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
            if(true) /* comments */ Console.WriteLine(""true"");
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
            if(true) /* comments */
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, IfStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_WithoutBraces_OnNextLine_InvokesWarning()
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
            if(true)
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, IfStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_WithBraces_DoesNotInvokeWarning()
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_ElseStatementWithoutBraces_OnSameLine_InvokesWarning()
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
            else Console.WriteLine(""false"");
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
            else
            {
                Console.WriteLine(""false"");
            }
        }
    }
}";

            VerifyDiagnostic(original, IfStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_ElseStatementWithoutBraces_OnNextLine_InvokesWarning()
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
            else 
                Console.WriteLine(""false"");
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
            else
            {
                Console.WriteLine(""false"");
            }
        }
    }
}";

            VerifyDiagnostic(original, IfStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_ElseStatementWithBraces_DoesNotInvokeWarning()
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
            else 
            {
                Console.WriteLine(""false"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void IfStatementWithoutBracesAnalyzer_IfAndElseWithoutBraces_InvokesWarning()
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
            if(true)
                Console.WriteLine(""true"");
            else
                Console.WriteLine(""false"");
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
            if(true)
            {
                Console.WriteLine(""true"");
            }
            else
            {
                Console.WriteLine(""false"");
            }
        }
    }
}";

            VerifyDiagnostic(original,
                IfStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString(),
                IfStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}