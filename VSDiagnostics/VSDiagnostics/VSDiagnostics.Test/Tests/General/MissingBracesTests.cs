using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.MissingBraces;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class MissingBracesTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new MissingBracesAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new MissingBracesCodeFix();

        [TestMethod]
        public void MissingBraces_IfWithoutBraces_OnSameLine()
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

            VerifyDiagnostic(original, "An if statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_IfWithoutBraces_OnSameLine_WithIntermittentComment()
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

            VerifyDiagnostic(original, "An if statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_IfWithoutBraces_OnNextLine()
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

            VerifyDiagnostic(original, "An if statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_IfWithBraces()
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
        public void MissingBraces_ElseStatementWithoutBraces_OnSameLine()
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

            VerifyDiagnostic(original, "An else statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_ElseStatementWithoutBraces_OnNextLine()
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

            VerifyDiagnostic(original, "An else statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_ElseStatementWithBraces()
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
        public void MissingBraces_IfAndElseWithoutBraces()
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
                "An if statement should be written with braces",
                "An else statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_ElseIfWithBraces()
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
            var i = 5;
            if (i == 5)
            {
                Console.WriteLine(""true"");
            }
            else if (i == 4)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingBraces_ElseIfWithoutBraces()
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
            var i = 5;
            if (i == 5)
            {
                Console.WriteLine(""true"");
            }
            else if (i == 4)
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
            var i = 5;
            if (i == 5)
            {
                Console.WriteLine(""true"");
            }
            else if (i == 4)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "An if statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_UsingWithoutBraces_OnSameLine()
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

            VerifyDiagnostic(original, "A using statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_UsingWithoutBraces_OnSameLine_WithIntermittentComment()
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

            VerifyDiagnostic(original, "A using statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_UsingWithoutBraces_OnNextLine()
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

            VerifyDiagnostic(original, "A using statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_UsingWithBraces()
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
        public void MissingBraces_WithOutBraces_ChildIsUsingStatement()
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
        public void MissingBraces_WithOutBraces_ChildIsUsingStatementWithBraces()
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

            VerifyDiagnostic(original, "A using statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_For_WithoutBraces_OnSameLine()
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
            for (int i = 0; i < 10; i++) Console.WriteLine(""true"");
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
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A for statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_For_WithoutBraces_OnSameLine_WithIntermittentComment()
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
            for (int i = 0; i < 10; i++) /* comments */ Console.WriteLine(""true"");
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
            for (int i = 0; i < 10; i++) /* comments */
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A for statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_For_WithoutBraces_OnNextLine()
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
            for (int i = 0; i < 10; i++)
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
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A for statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_For_WithBraces()
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
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingBraces_While_WithoutBraces_OnSameLine()
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
            while (true) Console.WriteLine(""true"");
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
            while (true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A while statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_While_WithoutBraces_OnSameLine_WithIntermittentComment()
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
            while (true) /* comments */ Console.WriteLine(""true"");
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
            while (true) /* comments */
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A while statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_While_WithoutBraces_OnNextLine()
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
            while (true)
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
            while (true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A while statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_While_WithBraces()
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
            while (true)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingBraces_Foreach_WithoutBraces_OnSameLine()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list) Console.WriteLine(""true"");
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A foreach statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_Foreach_WithoutBraces_OnSameLine_WithIntermittentComment()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list) /* comments */ Console.WriteLine(""true"");
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list) /* comments */
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A foreach statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_Foreach_WithoutBraces_OnNextLine()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
                Console.WriteLine(""true"");
        }
    }
}";

            var result = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";

            VerifyDiagnostic(original, "A foreach statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_Foreach_WithBraces()
        {
            var original = @"
using System;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class MyClass
    {   
        void Method()
        {
            var list = new List<int>();
            foreach (var item in list)
            {
                Console.WriteLine(""true"");
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingBraces_DoWhile_WithoutBraces_OnSameLine()
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
            do Console.WriteLine(""true""); while (true);
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
            do
            {
                Console.WriteLine(""true"");
            }
            while (true);
        }
    }
}";

            VerifyDiagnostic(original, "A do statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_DoWhile_WithoutBraces_OnSameLine_WithIntermittentComment()
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
            do /* comments */
                Console.WriteLine(""true"");
            while (true);
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
            do /* comments */
            {
                Console.WriteLine(""true"");
            }
            while (true);
        }
    }
}";

            VerifyDiagnostic(original, "A do statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_DoWhile_WithoutBraces_OnNextLine()
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
            do
                Console.WriteLine(""true"");
            while (true);
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
            do
            {
                Console.WriteLine(""true"");
            }
            while (true);
        }
    }
}";

            VerifyDiagnostic(original, "A do statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_DoWhile_WithBraces()
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
            do
            {
                Console.WriteLine(""true"");
            } while (true);
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingBraces_Lock_WithoutBraces()
        {
            var original = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        lock (str)
            return;
    }
}";

            var result = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        lock (str)
        {
            return;
        }
    }
}";

            VerifyDiagnostic(original, "A lock statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_Lock_WithoutBraces_NestedInLock()
        {
            var original = @"
class Program
{
    static void Main()
    {
        var str1 = ""test"";
        var str2 = ""test"";
        lock (str1)
        lock (str2) // VS thinks this should be indented one more level
            return;
    }
}";

            var result = @"
class Program
{
    static void Main()
    {
        var str1 = ""test"";
        var str2 = ""test"";
        lock (str1)
        lock (str2) // VS thinks this should be indented one more level
            {
                return;
            }
    }
}";

            VerifyDiagnostic(original, "A lock statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_Lock_WithBraces()
        {
            var original = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        lock (str)
        {
            return;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [Ignore]    // need unsafe compilation option
        [TestMethod]
        public void MissingBraces_Fixed_WithoutBraces()
        {
            var original = @"
class Point 
{ 
    public int x;
    public int y; 
}

class Program
{
    unsafe static void TestMethod()
    {
        var pt = new Point();
        fixed (int* p = &pt.x)
            *p = 1;
    }
}";

            var result = @"
class Point 
{ 
    public int x;
    public int y; 
}

class Program
{
    unsafe static void TestMethod()
    {
        var pt = new Point();
        fixed (int* p = &pt.x)
        {
            *p = 1;
        }
    }
}";

            VerifyDiagnostic(original, "A fixed statement should be written with braces");
            VerifyFix(original, result);
        }

        [Ignore]    // need unsafe compilation option
        [TestMethod]
        public void MissingBraces_Fixed_WithoutBraces_NestedInFixed()
        {
            var original = @"
class Point 
{ 
    public int x;
    public int y; 
}

class Program
{
    unsafe static void TestMethod()
    {
        var pt = new Point();
        fixed (int* p = &pt.x)
        fixed (int* q = &pt.y)
            *p = 1;
    }
}";

            var result = @"
class Point 
{ 
    public int x;
    public int y; 
}

class Program
{
    unsafe static void TestMethod()
    {
        var pt = new Point();
        fixed (int* p = &pt.x)
        fixed (int* q = &pt.y)
        {
            *p = 1;
        }
    }
}";

            VerifyDiagnostic(original, "A fixed statement should be written with braces");
            VerifyFix(original, result);
        }

        [Ignore]    // need unsafe compilation option
        [TestMethod]
        public void MissingBraces_Fixed_WithBraces()
        {
            var original = @"
class Point 
{ 
    public int x;
    public int y; 
}

class Program
{
    unsafe static void TestMethod()
    {
        var pt = new Point();
        fixed (int* p = &pt.x)
        {
            *p = 1;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingBraces_CaseLabel_WithoutBraces()
        {
            var original = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        switch (str)
        {
            case ""test"":
                return;
        }
    }
}";

            var result = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        switch (str)
        {
            case ""test"":
                {
                    return;
                }
        }
    }
}";

            VerifyDiagnostic(original, "A switch section statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_CaseLabel_WithoutBraces_ManyChildStatements()
        {
            var original = @"
class Program
{
    static int Test()
    {
        var str = ""test"";
        switch (str)
        {
            case ""test"":
                var i = 0;
                i = str.GetHashCode();
                return i;
        }
        return 0;
    }
}";

            var result = @"
class Program
{
    static int Test()
    {
        var str = ""test"";
        switch (str)
        {
            case ""test"":
                {
                    var i = 0;
                    i = str.GetHashCode();
                    return i;
                }
        }
        return 0;
    }
}";

            VerifyDiagnostic(original, "A switch section statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_DefaultLabel_WithoutBraces()
        {
            var original = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        switch (str)
        {
            default:
                return;
        }
    }
}";

            var result = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        switch (str)
        {
            default:
                {
                    return;
                }
        }
    }
}";

            VerifyDiagnostic(original, "A switch section statement should be written with braces");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void MissingBraces_CaseLabel_WithBraces()
        {
            var original = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        switch (str)
        {
            case ""test"":
            {
                return;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void MissingBraces_DefaultLabel_WithBraces()
        {
            var original = @"
class Program
{
    static void Main()
    {
        var str = ""test"";
        switch (str)
        {
            default:
            {
                return;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}