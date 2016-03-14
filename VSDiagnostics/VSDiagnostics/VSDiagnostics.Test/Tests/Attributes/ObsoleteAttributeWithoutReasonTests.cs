using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.ObsoleteAttributeWithoutReason;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class ObsoleteAttributeWithoutReasonTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ObsoleteAttributeWithoutReasonAnalyzer();

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithNullArgumentList()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithoutReason()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [ObsoleteAttribute]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithEmptyArgumentList()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete()]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithEmptyArgumentList()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [ObsoleteAttribute()]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test, ObsoleteAttributeWithoutReasonAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithArgument()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete(""I have an argument."")]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithArgument()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [ObsoleteAttribute(""I have an argument."")]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteWithArguments()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [Obsolete(""I have two arguments."", true)]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_WithObsoleteAttributeWithArguments()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [ObsoleteAttribute(""I have two arguments."", true)]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test);
        }

        [TestMethod]
        public void ObsoleteAttributeWithoutReason_NonObsoleteAttribute()
        {
            var test = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        [MTAThread]
        void Method(string input)
        {
        }
    }
}";

            VerifyDiagnostic(test);
        }
    }
}