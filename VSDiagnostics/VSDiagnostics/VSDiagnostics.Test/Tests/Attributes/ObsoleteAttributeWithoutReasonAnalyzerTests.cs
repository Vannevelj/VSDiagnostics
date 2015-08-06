﻿using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.ObsoleteAttributeWithoutReason;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class ObsoleteAttributeWithoutReasonAnalyzerTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ObsoleteAttributeWithoutReasonAnalyzer();

        [TestMethod]
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteWithNullArgumentList_InvokesWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteAttributeWithoutReason_InvokesWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteWithEmptyArgumentList_InvokesWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteAttributeWithEmptyArgumentList_InvokesWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteWithArgument_DoesNotInvokeWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteAttributeWithArgument_DoesNotInvokeWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteWithArguments_DoesNotInvokeWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_WithObsoleteAttributeWithArguments_DoesNotInvokeWarning()
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
        public void ObsoleteAttributeWithoutReasonAnalyzer_NonObsoleteAttribute_DoesNotInvokeWarning()
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