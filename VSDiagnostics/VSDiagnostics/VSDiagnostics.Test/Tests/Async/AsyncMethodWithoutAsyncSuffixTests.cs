using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Async.AsyncMethodWithoutAsyncSuffix;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class AsyncMethodWithoutAsyncSuffixTests : CSharpCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider => new AsyncMethodWithoutAsyncSuffixCodeFix();

        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AsyncMethodWithoutAsyncSuffixAnalyzer();

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithAsyncKeywordAndNoSuffix()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task Method()
            {
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
            async Task MethodAsync()
            {
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithAsyncKeywordAndSuffix_DoesNotDisplayWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async Task MethodAsync()
            {
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithoutAsyncKeywordAndSuffix_DoesNotDisplayWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void Method()
            {
            }
        }
    }";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            Task Method()
            {
                return Task.CompletedTask;
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
            Task MethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithGenericTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            Task<int> Method()
            {
                return Task.FromResult(5);
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
            Task<int> MethodAsync()
            {
                return Task.FromResult(5);
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInInterface_WithTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethod();
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethodAsync();
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInInterface_WithGenericTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task<int> MyMethod();
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task<int> MyMethodAsync();
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInInterface_WithImplementedMember_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethod();
        }

        class MyClass : IMyInterface
        {
            public Task MyMethod()
            {
                return Task.CompletedTask;
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethodAsync();
        }

        class MyClass : IMyInterface
        {
            public Task MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInInterface_WithImplementedMember_AndAsyncModifier_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethod();
        }

        class MyClass : IMyInterface
        {
            public async Task MyMethod()
            {
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethodAsync();
        }

        class MyClass : IMyInterface
        {
            public async Task MyMethodAsync()
            {
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromAbstractMethod_WithAsyncModifier_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract Task MyMethod();
        }

        class MyClass : BaseClass
        {
            public override async Task MyMethod()
            {
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract Task MyMethodAsync();
        }

        class MyClass : BaseClass
        {
            public override async Task MyMethodAsync()
            {
            }
        }
    }";


            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromAbstractMethod_WithTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract Task MyMethod();
        }

        class MyClass : BaseClass
        {
            public override Task MyMethod()
            {
                return Task.CompletedTask;
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract Task MyMethodAsync();
        }

        class MyClass : BaseClass
        {
            public override Task MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromAbstractMethod_WithGenricTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract Task<int> MyMethod();
        }

        class MyClass : BaseClass
        {
            public override Task<int> MyMethod()
            {
                return Task.FromResult(5);
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract Task<int> MyMethodAsync();
        }

        class MyClass : BaseClass
        {
            public override Task<int> MyMethodAsync()
            {
                return Task.FromResult(5);
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromVirtualMethod_WithAsyncModifier_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual Task MyMethod()
            {
                return Task.FromResult(5);
            }
        }

        class MyClass : BaseClass
        {
            public override async Task MyMethod()
            {
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual Task MyMethodAsync()
            {
                return Task.FromResult(5);
            }
        }

        class MyClass : BaseClass
        {
            public override async Task MyMethodAsync()
            {
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromVirtualMethod_WithTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual Task MyMethod()
            {
                return Task.FromResult(10);
            }
        }

        class MyClass : BaseClass
        {
            public override Task MyMethod()
            {
                return Task.FromResult(5);
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual Task MyMethodAsync()
            {
                return Task.FromResult(10);
            }
        }

        class MyClass : BaseClass
        {
            public override Task MyMethodAsync()
            {
                return Task.FromResult(5);
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromVirtualMethod_WithGenericTaskReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual Task<int> MyMethod()
            {
                return Task.FromResult(5);
            }
        }

        class MyClass : BaseClass
        {
            public override Task<int> MyMethod()
            {
                return Task.FromResult(8);
            }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual Task<int> MyMethodAsync()
            {
                return Task.FromResult(5);
            }
        }

        class MyClass : BaseClass
        {
            public override Task<int> MyMethodAsync()
            {
                return Task.FromResult(8);
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithClasses_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseBaseClass
        {
	        public virtual Task MyMethod()
	        {
		        return Task.CompletedTask;
	        }
        }

        class BaseClass : BaseBaseClass
        {
	        public override Task MyMethod()
	        {
		        return Task.CompletedTask;
	        }
        }

        class MyClass : BaseClass
        {
	        public override Task MyMethod()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseBaseClass
        {
	        public virtual Task MyMethodAsync()
	        {
		        return Task.CompletedTask;
	        }
        }

        class BaseClass : BaseBaseClass
        {
	        public override Task MyMethodAsync()
	        {
		        return Task.CompletedTask;
	        }
        }

        class MyClass : BaseClass
        {
	        public override Task MyMethodAsync()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithLongerInheritanceStructure_WithInterfaces_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IBaseInterface
        {
            Task MyMethod();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : IMyInterface
        {
	        public Task MyMethod()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IBaseInterface
        {
            Task MyMethodAsync();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : IMyInterface
        {
	        public Task MyMethodAsync()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithInterfacesAndClasses_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
	        public abstract Task MyMethod();
        }

        interface IBaseInterface
        {
	        Task MyMethod();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : BaseClass, IMyInterface
        {
	        public override Task MyMethod()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
	        public abstract Task MyMethodAsync();
        }

        interface IBaseInterface
        {
	        Task MyMethodAsync();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : BaseClass, IMyInterface
        {
	        public override Task MyMethodAsync()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            VerifyDiagnostic(
                original,
                string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithMultipleInterfaces_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethod();
        }

        interface IAnotherInterface
        {
            Task MyMethod();
        }

        class MyClass : IMyInterface, IAnotherInterface
        {
	        public Task MyMethod()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            Task MyMethodAsync();
        }

        interface IAnotherInterface
        {
            Task MyMethodAsync();
        }

        class MyClass : IMyInterface, IAnotherInterface
        {
	        public Task MyMethodAsync()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            VerifyDiagnostic(
                original,
                string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_WithVoidReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            async void Method()
            {

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
            async void MethodAsync()
            {

            }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "Method"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithMultipleAbstractClasses_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
	        public abstract Task MyMethod();
        }

        abstract class BaseClass : OtherBaseClass
        {
	        
        }

        class MyClass : BaseClass
        {
	        public override Task MyMethod()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
	        public abstract Task MyMethodAsync();
        }

        abstract class BaseClass : OtherBaseClass
        {
	        
        }

        class MyClass : BaseClass
        {
	        public override Task MyMethodAsync()
	        {
		        return Task.CompletedTask;
	        }
        }
    }";

            VerifyDiagnostic(original, string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result);
        }

        /// <summary>
        ///     This should not display a warning because hidden methods are not considered the same by Roslyn.
        ///     You will also notice that when you rename a hidden method in VS2015, it will only rename one of the two methods.
        ///     Therefore, in this scenario, we will have two warnings instead of just one.
        /// </summary>
        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithHiddenMember()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
            public virtual Task MyMethod()
	        {
		        return null;
	        }
        }

        abstract class BaseClass : OtherBaseClass
        {
            public new Task MyMethod()
	        {
		        return null;
	        }
        }
    }";

            var result = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
            public virtual Task MyMethodAsync()
	        {
		        return null;
	        }
        }

        abstract class BaseClass : OtherBaseClass
        {
            public new Task MyMethodAsync()
	        {
		        return null;
	        }
        }
    }";

            VerifyDiagnostic(
                original, 
                string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"), 
                string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true); // CS0109 We're no longer hiding a base member so new becomes obsolete
        }
    }
}