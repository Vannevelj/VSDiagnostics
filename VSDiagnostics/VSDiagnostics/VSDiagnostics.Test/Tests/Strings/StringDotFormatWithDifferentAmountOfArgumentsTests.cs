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
        public void StringDotFormatWithDifferentAmountOfArguments_WithFormatProvider__AndFormat_AndNoArguments()
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
        public void StringDotFormatWithDifferentAmountOfArguments_WithSimilarInvocation()
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

        void Method(string s, int x)
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
    }
}
