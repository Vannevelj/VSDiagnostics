using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.SwitchDoesNotHandleAllEnumOptions;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class SwitchDoesNotHandleAllEnumOptionsTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SwitchDoesNotHandleAllEnumOptionsAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new SwitchDoesNotHandleAllEnumOptionsCodeFix();

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_MissingEnumStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.FizzBuzz:
                    throw new System.NotImplementedException();
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_AllEnumStatements()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                case MyEnum.FizzBuzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_CaseStatementsNotEnum()
        {
            var original = @"
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            switch (""test"")
            {
                case ""Fizz"":
                case ""Buzz"":
                case ""FizzBuzz"":
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_CaseHasDefaultStatement_MissingEnumStatement()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                default:
                    break;
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.FizzBuzz:
                    throw new System.NotImplementedException();
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                default:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_MissingEnumStatement_MultipleSections()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                    break;
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.FizzBuzz:
                    throw new System.NotImplementedException();
                case MyEnum.Fizz:
                    break;
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_UsingStaticEnum_MissingEnumStatements()
        {
            var original = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case Asynchronous:
                case DeleteOnClose:
                    break;
            }
        }
    }
}";

            var result = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case Encrypted:
                    throw new System.NotImplementedException();
                case SequentialScan:
                    throw new System.NotImplementedException();
                case RandomAccess:
                    throw new System.NotImplementedException();
                case WriteThrough:
                    throw new System.NotImplementedException();
                case None:
                    throw new System.NotImplementedException();
                case Asynchronous:
                case DeleteOnClose:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_MissingEnumStatement_AddsAllMissingStatements()
        {
            var original = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Buzz:
                    break;
                default:
                    break;
            }
        }
    }
}";

            var result = @"
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.FizzBuzz:
                    throw new System.NotImplementedException();
                case MyEnum.Fizz:
                    throw new System.NotImplementedException();
                case MyEnum.Buzz:
                    break;
                default:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_UsingStaticEnum_NoEnumStatements()
        {
            var original = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
            }
        }
    }
}";

            var result = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case Encrypted:
                    throw new System.NotImplementedException();
                case SequentialScan:
                    throw new System.NotImplementedException();
                case DeleteOnClose:
                    throw new System.NotImplementedException();
                case RandomAccess:
                    throw new System.NotImplementedException();
                case Asynchronous:
                    throw new System.NotImplementedException();
                case WriteThrough:
                    throw new System.NotImplementedException();
                case None:
                    throw new System.NotImplementedException();
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_UsingStaticEnum_MissingEnumStatement_MixedExpandedEnumStatements()
        {
            var original = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case FileOptions.Encrypted:
                    break;
                case SequentialScan:
                    break;
                case RandomAccess:
                    break;
            }
        }
    }
}";

            var result = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case DeleteOnClose:
                    throw new System.NotImplementedException();
                case Asynchronous:
                    throw new System.NotImplementedException();
                case WriteThrough:
                    throw new System.NotImplementedException();
                case None:
                    throw new System.NotImplementedException();
                case Encrypted:
                    break;
                case SequentialScan:
                    break;
                case RandomAccess:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result, allowedNewCompilerDiagnosticsId: "CS8019");     // unneeded using directive
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_UsingStaticEnum_MissingEnumStatement_AllExpandedEnumStatements()
        {
            var original = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case FileOptions.Encrypted:
                    break;
                case FileOptions.SequentialScan:
                    break;
                case FileOptions.RandomAccess:
                    break;
            }
        }
    }
}";

            var result = @"
using System.IO;
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case FileOptions.DeleteOnClose:
                    throw new System.NotImplementedException();
                case FileOptions.Asynchronous:
                    throw new System.NotImplementedException();
                case FileOptions.WriteThrough:
                    throw new System.NotImplementedException();
                case FileOptions.None:
                    throw new System.NotImplementedException();
                case FileOptions.Encrypted:
                    break;
                case FileOptions.SequentialScan:
                    break;
                case FileOptions.RandomAccess:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_MissingEnumStatement_NoRedundantQualifierIfUsingSystemDirectiveExists()
        {
            var original = @"
using System;
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            var result = @"
using System;
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.FizzBuzz:
                    throw new NotImplementedException();
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_MissingEnumStatement_UsingAliasForSystem()
        {
            var original = @"
using Fizz = System;    // seriously...
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            var result = @"
using Fizz = System;    // seriously...
namespace ConsoleApplication1
{
    enum MyEnum
    {
        Fizz, Buzz, FizzBuzz
    }

    class MyClass
    {
        void Method()
        {
            var e = MyEnum.Fizz;
            switch (e)
            {
                case MyEnum.FizzBuzz:
                    throw new Fizz.NotImplementedException();
                case MyEnum.Fizz:
                case MyEnum.Buzz:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SwitchDoesNotHandleAllEnumOptions_UsingStaticEnum_MissingEnumStatement_SimplifiesAllStatementsWhenParentDirectiveNotIncluded()
        {
            var original = @"
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case Encrypted:
                    break;
                case SequentialScan:
                    break;
                case RandomAccess:
                    break;
            }
        }
    }
}";

            var result = @"
using static System.IO.FileOptions;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var e = DeleteOnClose;
            switch (e)
            {
                case DeleteOnClose:
                    throw new System.NotImplementedException();
                case Asynchronous:
                    throw new System.NotImplementedException();
                case WriteThrough:
                    throw new System.NotImplementedException();
                case None:
                    throw new System.NotImplementedException();
                case Encrypted:
                    break;
                case SequentialScan:
                    break;
                case RandomAccess:
                    break;
            }
        }
    }
}";

            VerifyDiagnostic(original, SwitchDoesNotHandleAllEnumOptionsAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}