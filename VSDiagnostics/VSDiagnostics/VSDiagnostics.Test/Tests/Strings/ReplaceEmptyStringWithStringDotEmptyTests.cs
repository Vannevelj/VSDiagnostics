using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Strings.ReplaceEmptyStringWithStringDotEmpty;

namespace VSDiagnostics.Test.Tests.Strings
{
    [TestClass]
    public class ReplaceEmptyStringWithStringDotEmptyTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ReplaceEmptyStringWithStringDotEmptyAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new ReplaceEmptyStringWithStringDotEmptyCodeFix();

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithLocalEmptyStringLiteral_InvokesWarning()
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
            string s = """";
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
            string s = string.Empty;
        }
    }
}";

            VerifyDiagnostic(original, ReplaceEmptyStringWithStringDotEmptyAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithDefaultParameterEmptyStringLiteral_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(string s = """")
        {
                
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithNonEmptyStringLiteral_DoesNotInvokeWarning()
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
            string s = ""hello world"";
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithStringLiteralAsArgument_InvokesWarning()
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
            Method2("""");
        }

        void Method2(string s)
        {

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
            Method2(string.Empty);
        }

        void Method2(string s)
        {

        }
    }
}";

            VerifyDiagnostic(original, ReplaceEmptyStringWithStringDotEmptyAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithStringDotEmpty_DoesNotInvokeWarning()
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
            string s = string.Empty;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithEmptyStringAsAttributeArgument_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        [MyAttribute(Test = """")]
        void Method()
        {

        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class MyAttribute : Attribute
    {
	    public string Test { get; set; }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_WithConstField_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private const string x = """";
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ReplaceEmptyStringsWithStringDotEmpty_KeepsCleanFormatting_InvokesWarning()
        {
            var original = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    public class Foo
    {
        private List<Cookie> Goo()
        {
            var entries = new List<string>();

            return entries.Select(
                      x => x.Split(new[] { ""="" }, 2, StringSplitOptions.RemoveEmptyEntries))
                      .Select(pairs => new Cookie
                      {
                          Key = pairs[0].Trim(),
                          Value = pairs.Length > 1 ? pairs[1].Trim() : """"
                      }).ToList();
        }
    }

    internal class Cookie
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}";

            var result = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    public class Foo
    {
        private List<Cookie> Goo()
        {
            var entries = new List<string>();

            return entries.Select(
                      x => x.Split(new[] { ""="" }, 2, StringSplitOptions.RemoveEmptyEntries))
                      .Select(pairs => new Cookie
                      {
                          Key = pairs[0].Trim(),
                          Value = pairs.Length > 1 ? pairs[1].Trim() : string.Empty
                      }).ToList();
        }
    }

    internal class Cookie
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}";

            VerifyDiagnostic(original, ReplaceEmptyStringWithStringDotEmptyAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}