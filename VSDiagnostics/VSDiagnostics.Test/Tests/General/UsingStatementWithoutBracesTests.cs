using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.UsingStatementWithoutBraces;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class UsingStatementWithoutBracesTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new UsingStatementWithoutBracesAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new UsingStatementWithoutBracesCodeFix();

        [TestMethod]
        public void UsingStatementWithoutBraces_WithoutBraces_OnSameLine()
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
            using (var d = new Disposable()) Console.WriteLine(""true"");
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
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
            using (var d = new Disposable())
            {
                Console.WriteLine(""true"");
            }
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, UsingStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void UsingStatementWithoutBraces_WithoutBraces_OnSameLine_WithIntermittentComment()
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
            using (var d = new Disposable()) /* blah blah */ Console.WriteLine(""true"");
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
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
            using (var d = new Disposable()) /* blah blah */
            {
                Console.WriteLine(""true"");
            }
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, UsingStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void UsingStatementWithoutBraces_WithoutBraces_OnNextLine()
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
            using (var d = new Disposable())
                Console.WriteLine(""true"");
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
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
            using (var d = new Disposable())
            {
                Console.WriteLine(""true"");
            }
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, UsingStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void UsingStatementWithoutBraces_WithBraces()
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
            using (var d = new Disposable())
            {
                Console.WriteLine(""true"");
            }
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UsingStatementWithoutBraces_WithOutBraces_ChildIsUsingStatement()
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
            using (var d = new Disposable())
            using (var e = new Disposable())
            {
                Console.WriteLine(""true"");
            }
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void UsingStatementWithoutBraces_WithOutBraces_ChildIsUsingStatementWithBraces()
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
            using (var d = new Disposable())
            using (var e = new Disposable())
                Console.WriteLine(""true"");
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
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
            using (var d = new Disposable())
            using (var e = new Disposable())
            {
                Console.WriteLine(""true"");
            }
        }
    }

    class Disposable : IDisposable
    {   
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, UsingStatementWithoutBracesAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}