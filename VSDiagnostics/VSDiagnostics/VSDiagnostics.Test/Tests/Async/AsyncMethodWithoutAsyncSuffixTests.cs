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
        public void AsyncMethodWithoutAsyncSuffix_WithAsyncKeywordAndNoSuffix_InvokesWarning()
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
            async Task MethodAsync()
            {
                return Task.CompletedTask;
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
                return Task.CompletedTask;
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
                return Task.CompletedTask;
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

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 18)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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
            public async Task MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 18)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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
            public override async Task MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 34)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 34)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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
            public abstract Task<int> MyMethodAsync();
        }

        class MyClass : BaseClass
        {
            public override Task<int> MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 39)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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
                return Task.CompletedTask;
            }
        }

        class MyClass : BaseClass
        {
            public override async Task MyMethod()
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
        class BaseClass
        {
            public virtual Task MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }

        class MyClass : BaseClass
        {
            public override async Task MyMethodAsync()
            {
                return Task.CompletedTask;
            }
        }
    }";

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 33)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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
        class BaseClass
        {
            public virtual Task MyMethodAsync()
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

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 33)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 38)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 30)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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

            var diagnosticResult = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 18)
                }
            };

            VerifyDiagnostic(original, diagnosticResult);
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

            var diagnosticResultClass = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 31)
                }
            };

            var diagnosticResultInterface = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 15, 15)
                }
            };

            VerifyDiagnostic(original, diagnosticResultClass, diagnosticResultInterface);
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

            var diagnosticResultClass = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 18)
                }
            };

            var diagnosticResultInterface = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 15, 18)
                }
            };

            VerifyDiagnostic(original, diagnosticResultClass, diagnosticResultInterface);
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

            var diagnosticResultClass = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 31)
                }
            };

            VerifyDiagnostic(original, diagnosticResultClass);
            VerifyFix(original, result);
        }

        [TestMethod]
        public void AsyncMethodWithoutAsyncSuffix_DefinedInBaseClass_WithHiddenMember_InvokesWarning()
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

            var diagnosticResultClass = new DiagnosticResult
            {
                Id = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.Id,
                Message = string.Format(AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.MessageFormat.ToString(), "MyMethod"),
                Severity = AsyncMethodWithoutAsyncSuffixAnalyzer.Rule.DefaultSeverity,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", 10, 31)
                }
            };

            VerifyDiagnostic(original, diagnosticResultClass);
            VerifyFix(original, result);
        }
    }
}