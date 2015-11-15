using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Async.SyncMethodWithSyncSuffix;

namespace VSDiagnostics.Test.Tests.Async
{
    [TestClass]
    public class SyncMethodWithAsyncSuffixTests : CSharpCodeFixVerifier
    {
        protected override CodeFixProvider CodeFixProvider => new SyncMethodWithAsyncSuffixCodeFix();

        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new SyncMethodWithAsyncSuffixAnalyzer();

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_WithoutAsyncKeywordAndNoSuffix_DoesNotDisplayWarning()
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
        public void SyncMethodWithAsyncSuffix_WithoutAsyncKeywordAndSuffix_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class MyClass
        {   
            void MethodAsync()
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
            void Method()
            {
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_WithAsyncKeywordAndSuffix_DoesNotDisplayWarning()
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
        public void SyncMethodWithAsyncSuffix_DefinedInInterface_WithVoidReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            void MyMethodAsync();
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
            void MyMethod();
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
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
            void MyMethodAsync();
        }

        class MyClass : IMyInterface
        {
            public void MyMethodAsync()
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
            void MyMethod();
        }

        class MyClass : IMyInterface
        {
            public void MyMethod()
            {
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromAbstractMethod_WithVoidReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
            public abstract void MyMethodAsync();
        }

        class MyClass : BaseClass
        {
            public override void MyMethodAsync()
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
            public abstract void MyMethod();
        }

        class MyClass : BaseClass
        {
            public override void MyMethod()
            {
            }
        }
    }";
            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_FromVirtualMethod_WithVoidReturnType_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseClass
        {
            public virtual void MyMethodAsync()
            {
            }
        }

        class MyClass : BaseClass
        {
            public override void MyMethodAsync()
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
            public virtual void MyMethod()
            {
            }
        }

        class MyClass : BaseClass
        {
            public override void MyMethod()
            {
            }
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithClasses_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        class BaseBaseClass
        {
	        public virtual void MyMethodAsync()
	        {
	        }
        }

        class BaseClass : BaseBaseClass
        {
	        public override void MyMethodAsync()
	        {
	        }
        }

        class MyClass : BaseClass
        {
	        public override void MyMethodAsync()
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
        class BaseBaseClass
        {
	        public virtual void MyMethod()
	        {
	        }
        }

        class BaseClass : BaseBaseClass
        {
	        public override void MyMethod()
	        {
	        }
        }

        class MyClass : BaseClass
        {
	        public override void MyMethod()
	        {
	        }
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithLongerInheritanceStructure_WithInterfaces_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IBaseInterface
        {
            void MyMethodAsync();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : IMyInterface
        {
	        public void MyMethodAsync()
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
        interface IBaseInterface
        {
            void MyMethod();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : IMyInterface
        {
	        public void MyMethod()
	        {
	        }
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithInterfacesAndClasses_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class BaseClass
        {
	        public abstract void MyMethodAsync();
        }

        interface IBaseInterface
        {
	        void MyMethodAsync();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : BaseClass, IMyInterface
        {
	        public override void MyMethodAsync()
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
	        public abstract void MyMethod();
        }

        interface IBaseInterface
        {
	        void MyMethod();
        }

        interface IMyInterface : IBaseInterface
        {

        }

        class MyClass : BaseClass, IMyInterface
        {
	        public override void MyMethod()
	        {
	        }
        }
    }";

            VerifyDiagnostic(
                original,
                string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"),
                string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithMultipleInterfaces_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        interface IMyInterface
        {
            void MyMethodAsync();
        }

        interface IAnotherInterface
        {
            void MyMethodAsync();
        }

        class MyClass : IMyInterface, IAnotherInterface
        {
	        public void MyMethodAsync()
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
            void MyMethod();
        }

        interface IAnotherInterface
        {
            void MyMethod();
        }

        class MyClass : IMyInterface, IAnotherInterface
        {
	        public void MyMethod()
	        {
	        }
        }
    }";

            VerifyDiagnostic(
                original,
                string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"),
                string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithOverriddenMember_WithLongerInheritanceStructure_WithMultipleAbstractClasses_InvokesWarning()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
	        public abstract void MyMethodAsync();
        }

        abstract class BaseClass : OtherBaseClass
        {
	        
        }

        class MyClass : BaseClass
        {
	        public override void MyMethodAsync()
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
        abstract class OtherBaseClass
        {
	        public abstract void MyMethod();
        }

        abstract class BaseClass : OtherBaseClass
        {
	        
        }

        class MyClass : BaseClass
        {
	        public override void MyMethod()
	        {
	        }
        }
    }";

            VerifyDiagnostic(original, string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result);
        }

        /// <summary>
        ///     This should have two warnings (<seealso cref="AsyncMethodWithoutAsyncSuffixTests.AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithHiddenMember"/>).
        /// </summary>
        [TestMethod]
        public void SyncMethodWithAsyncSuffix_DefinedInBaseClass_WithHiddenMember()
        {
            var original = @"
    using System;
    using System.Text;
    using System.Threading.Tasks;

    namespace ConsoleApplication1
    {
        abstract class OtherBaseClass
        {
            public virtual void MyMethodAsync()
	        {
	        }
        }

        abstract class BaseClass : OtherBaseClass
        {
            public new void MyMethodAsync()
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
        abstract class OtherBaseClass
        {
            public virtual void MyMethod()
	        {
	        }
        }

        abstract class BaseClass : OtherBaseClass
        {
            public new void MyMethod()
	        {
	        }
        }
    }";

            VerifyDiagnostic(
                original,
                string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"),
                string.Format(SyncMethodWithAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethodAsync"));
            VerifyFix(original, result, allowNewCompilerDiagnostics: true); // CS0109 We're no longer hiding a base member so new becomes obsolete
        }
    }
}