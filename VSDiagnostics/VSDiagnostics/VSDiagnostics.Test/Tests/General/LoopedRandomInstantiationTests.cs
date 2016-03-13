using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.LoopedRandomInstantiation;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class LoopedRandomInstantiationTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new LoopedRandomInstantiationAnalyzer();

        [TestMethod]
        public void LoopedRandomInstantiation_WhileLoop()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            while (true)
            {
                var rand = new Random();
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(string.Format(LoopedRandomInstantiationAnalyzer.Rule.MessageFormat.ToString(), "rand"), "rand"));
        }

        [TestMethod]
        public void LoopedRandomInstantiation_DoWhileLoop()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            do
            {
                var rand = new Random();
            } while (true);
        }
    }
}";

            VerifyDiagnostic(original, string.Format(LoopedRandomInstantiationAnalyzer.Rule.MessageFormat.ToString(), "rand"));
        }

        [TestMethod]
        public void LoopedRandomInstantiation_ForLoop()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            for (var i = 0; i > 5; i++)
            {
                var rand = new Random(4);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(LoopedRandomInstantiationAnalyzer.Rule.MessageFormat.ToString(), "rand"));
        }

        [TestMethod]
        public void LoopedRandomInstantiation_ForeachLoop()
        {
            var original = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<string>();
            foreach (var item in list)
            {
                var rand = new Random();
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(LoopedRandomInstantiationAnalyzer.Rule.MessageFormat.ToString(), "rand"));
        }

        [TestMethod]
        public void LoopedRandomInstantiation_MultipleDeclaratorsInDeclaration()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            while (true)
            {
                Random rand = new Random(), rind = new Random(2);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(LoopedRandomInstantiationAnalyzer.Rule.MessageFormat.ToString(), "rand"),
                                       string.Format(LoopedRandomInstantiationAnalyzer.Rule.MessageFormat.ToString(), "rind"));
        }

        [TestMethod]
        public void LoopedRandomInstantiation_MultipleLevelsOfNesting()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            while (true)
            {
                if (true)
                {
                    Random rand = new Random();
                }
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(LoopedRandomInstantiationAnalyzer.Rule.MessageFormat.ToString(), "rand"));
        }

        [TestMethod]
        public void LoopedRandomInstantiation_RandomInstanceNotInLoop()
        {
            var original = @"
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var rand = new Random();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void LoopedRandomInstantiation_RandomNotSystemRandom()
        {
            var original = @"
namespace ConsoleApplication1
{
    class Random {}

    class MyClass
    {
        void Method()
        {
            var rand = new Random();
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}
