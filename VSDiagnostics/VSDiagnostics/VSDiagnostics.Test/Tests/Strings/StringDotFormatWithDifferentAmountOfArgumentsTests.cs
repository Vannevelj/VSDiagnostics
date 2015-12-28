using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Strings.StringDotFormatWithDifferentAmountOfArguments;

namespace VSDiagnostics.Test.Tests.Strings
{
    [TestClass]
    public class StringDotFormatWithDifferentAmountOfArgumentsTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new StringDotFormatWithDifferentAmountOfArgumentsAnalyzer();

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithValidScenario()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}, def {1}"", 1, 2);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithRepeatedPlaceholders()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}, def {0}"", 1);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithExtraArguments()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}, def {1}"", 1, 2, 3, 4, 5);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithLackingArguments()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}, def {1}"", 1);
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithLackingArguments_AndSkippedPlaceholderIndex()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {1}, def {2}"", 123, 456);
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithEqualAmountOfPlaceholdersAndArgumentsButDontMatchUp()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}, def {0}"", 1, 2);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithEscapedPlaceholder()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}, def {{1}}"", 1);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithPlaceholderFormatting()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {1:00}, def {1}"", 1);
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_InDifferentOrder()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {1}, def {0}"", 1);
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithoutFormatLiteral()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string format = ""abc {0}, def {1}"";
            string s = string.Format(format, 1, 2);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithInterpolatedString()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string name = ""Jeroen"";
            string s = string.Format($""abc {name}, def {0} ghi {1}"", 1);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithInterpolatedString_AndCultureInfo()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string name = ""Jeroen"";
            string s = string.Format(CultureInfo.InvariantCulture, $""abc {name}, def {0} ghi {1}"", 1);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithFormatProvider()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(CultureInfo.InvariantCulture, ""def {0} ghi {1}"", 1);
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithFormatProvider_AndFormat_AndNoArguments()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(CultureInfo.InvariantCulture, ""abc {0}"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithEscapedBraces()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""def {0} ghi {{1}}"", 1);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithNestedBraces()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""{{def {0} ghi {1}}}"", 1);
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }


        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithSimilarInvocation_WithWrongTypes()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        MyClass()
        {
            Method(""{{def {0} ghi {1}}}"", 1);
        }

        void Method(string format, int x)
        {
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithSimilarInvocation_WithCorrectTypes()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        MyClass()
        {
            Method(""{{def {0} ghi {1}}}"", 1);
        }

        void Method(string format, object x)
        {
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithSimilarInvocation_WithWrongFormatParamName()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        MyClass()
        {
            Method(""{{def {0} ghi {1}}}"", 1);
        }

        void Method(string s, object x)
        {
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithoutArguments()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}, def {1}"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithoutPlaceholders()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc, def"");
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithExplicitArray()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0}"", new object[] {""hello""});
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithExplicitArrayAndLackingArguments()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0} {1}"", new object[] {""hello""});
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }


        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithExplicitArrayMultipleArguments()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0} {1}"", new object[] {""hello"", ""bye""});
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithExplicitArrayReferenced()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            object[] arr = new object[] {""hello"", ""bye""};
            string s = string.Format(""abc {0} {1}"", arr);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithExplicitArrayReferencedThroughMethodCall()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0} {1}"", getArguments());
        }

        object[] getArguments()
        {
            return new object[] {""hello"", ""bye""};
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithExplicitArrayAndAdditionalArguments()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(""abc {0} {1} {2}"", new object[] {""hello"", ""bye"", ""uhoh""}, ""test"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithFormatProvider_AndExplicitArray()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(CultureInfo.InvariantCulture, ""abc {0}"", new object[] {""hello""});
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithFormatProvider_AndExplicitArrayWithLackingArguments()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            string s = string.Format(CultureInfo.InvariantCulture, ""abc {0}{1}"", new object[] {""hello""});
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithFormatProvider_AndExplicitArrayReferencedThroughVariable()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            var args = new object[] {""hello""};
            string s = string.Format(CultureInfo.InvariantCulture, ""abc {0}{1}"", args);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithConstantFormat()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            const string format = ""{0}{1}"";
            string s = string.Format(format, ""arg"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithConstantConcatenation()
        {
            var original = @"
using System;
using System.Globalization;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            const string a = ""{0}"";
            const string b = ""{1}"";
            const string format = a + b;
            string s = string.Format(format, ""arg"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithStaticImport()
        {
            var original = @"
using static System.String;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            const string a = ""{0}"";
            const string b = ""{1}"";
            const string format = a + b;
            string s = Format(format, ""arg"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithConsoleWriteLine_AndSingleObject()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            Console.WriteLine(""{0}{1}"", ""arg"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithConsoleWriteLine_AndParamsObject()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            Console.WriteLine(""{0}{1}{2}"", ""arg"", ""arg2"");
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithConsoleWriteLine_AndObjectArray()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            Console.WriteLine(""{0}{1}{2}"", new object[] { ""arg"", ""arg2"" });
        }
    }
}";
            VerifyDiagnostic(original, StringDotFormatWithDifferentAmountOfArgumentsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void StringDotFormatWithDifferentAmountOfArguments_WithConsoleWriteLine_AndObjectArrayReference()
        {
            var original = @"
using System;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method(string input)
        {
            var args = new object[] { ""arg"", ""arg2"" };
            Console.WriteLine(""{0}{1}{2}"", args);
        }
    }
}";
            VerifyDiagnostic(original);
        }
    }
}