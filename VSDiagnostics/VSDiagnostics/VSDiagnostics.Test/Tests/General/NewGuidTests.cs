using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.NewGuid;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class NewGuidTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new NewGuidAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new NewGuidCodeFix();

        [TestMethod]
        public void NewGuid_Constructor_NewGuid()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var g = new Guid();
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
            var g = Guid.NewGuid();
        }
    }
}";

            VerifyDiagnostic(original, "An empty guid was created in an ambiguous manner");
            VerifyFix(original, result, codeFixIndex: 0);
        }

        [TestMethod]
        public void NewGuid_Constructor_NewGuid_FullName()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var g = new System.Guid();
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
            var g = System.Guid.NewGuid();
        }
    }
}";

            VerifyDiagnostic(original, "An empty guid was created in an ambiguous manner");
            VerifyFix(original, result, codeFixIndex: 0);
        }

        [TestMethod]
        public void NewGuid_Constructor_EmptyGuid()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var g = new Guid();
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
            var g = Guid.Empty;
        }
    }
}";

            VerifyDiagnostic(original, "An empty guid was created in an ambiguous manner");
            VerifyFix(original, result, codeFixIndex: 1);
        }

        [TestMethod]
        public void NewGuid_Constructor_EmptyGuid_FullName()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var g = new System.Guid();
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
            var g = System.Guid.Empty;
        }
    }
}";

            VerifyDiagnostic(original, "An empty guid was created in an ambiguous manner");
            VerifyFix(original, result, codeFixIndex: 1);
        }

        [TestMethod]
        public void NewGuid_Constructor_AsExpression()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Console.WriteLine(new Guid());
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
            Console.WriteLine(Guid.NewGuid());
        }
    }
}";

            VerifyDiagnostic(original, "An empty guid was created in an ambiguous manner");
            VerifyFix(original, result, codeFixIndex: 0);
        }

        [TestMethod]
        public void NewGuid_GuidNewGuid()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Guid g = Guid.NewGuid();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NewGuid_GuidEmpty()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Guid g = Guid.Empty;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void NewGuid_Default()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            Guid g = default(Guid);
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}
