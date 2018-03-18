using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Async.AsyncMethodWithVoidReturnType;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class AsyncMethodWithVoidReturnTypeTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AsyncMethodWithVoidReturnTypeAnalyzer();
        protected override CodeFixProvider CodeFixProvider => new AsyncMethodWithVoidReturnTypeCodeFix();

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithAsyncAndTask()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task MyMethod()
            {
               await Task.Run(() => { });
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_NoAsync()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void MyMethod()
            {

            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithAsyncAndTaskGeneric()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task<int> MyMethod()
            {
               return 32;
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithAsyncAndEventHandlerArguments()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async void MyHandler(object o, EventArgs e)
            {
               await Task.Run(() => { });
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithAsyncAndEventHandlerSubClassArguments()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async void MyHandler(object o, MyEventArgs e)
            {
               await Task.Run(() => { });
            }
        }

        class MyEventArgs : EventArgs 
        {

        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithAsyncDelegate()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            public void MyMethod()
            {
	            TestMethod(async () => await Task.Run(() => {}));
            }

            public void TestMethod(Action callback)
            {
	            callback();
            }
        }
    }";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithAsyncVoidAndArbitraryArguments()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async void MyHandler(object o, int e)
            {
               await Task.Run(() => { });
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task MyHandler(object o, int e)
            {
               await Task.Run(() => { });
            }
        }
    }";

            VerifyDiagnostic(original, "Method MyHandler is marked as async but has a void return type");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithAsyncAndVoid()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async void MyMethod()
            {
               await Task.Run(() => { });
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task MyMethod()
            {
               await Task.Run(() => { });
            }
        }
    }";

            VerifyDiagnostic(original, "Method MyMethod is marked as async but has a void return type");
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithVoidReturnType_WithPartialMethod()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        partial class A
        {
            partial void OnSomethingHappened();
        }

        partial class A
        {
            async partial void OnSomethingHappened()
            {
                await Task.Run(() => { });
            }
        }
    }";

            VerifyDiagnostic(original);
        }
    }
}
