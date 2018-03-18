using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.ElementaryMethodsOfTypeInCollectionNotOverridden;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class ElementaryMethodsOfTypeInCollectionNotOverriddenTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer();

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithReferenceType()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    class MyCollectionItem {}
}";

            VerifyDiagnostic(original, ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithInterfaceType()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    interface MyCollectionItem {}
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithValueType()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    struct MyCollectionItem {}
}";

            VerifyDiagnostic(original, ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithReferenceType_ImplementsEquals()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    class MyCollectionItem
    {
        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithValueType_ImplementsEquals()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    struct MyCollectionItem
    {
        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithReferenceType_ImplementsGetHashCode()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    class MyCollectionItem
    {
        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithValueType_ImplementsGetHashCode()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    struct MyCollectionItem
    {
        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original, ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithReferenceType_ImplementsMethods()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    class MyCollectionItem
    {
        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_WithValueType_ImplementsMethods()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new List<MyCollectionItem>();
        }
    }

    struct MyCollectionItem
    {
        public override bool Equals(object obj)
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_Dictionary_BothDoNotImplementMethods()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new Dictionary<MyCollectionItem, MyCollectionItem>();
        }
    }

    class MyCollectionItem {}
}";

            VerifyDiagnostic(original,
                ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString(),
                ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_Dictionary_OneDoesNotImplementMethods()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = new Dictionary<int, MyCollectionItem>();
        }
    }

    class MyCollectionItem {}
}";

            VerifyDiagnostic(original,
                ElementaryMethodsOfTypeInCollectionNotOverriddenAnalyzer.Rule.MessageFormat.ToString());
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_TypeParameterWithoutObjectCreation()
        {
            var original = @"
using System.Linq;
namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            var list = Enumerable.Empty<MyCollectionItem>();
        }
    }

    class MyCollectionItem {}
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_GenericTypeFromClass()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    internal class MyClass<T>
    {
        public static List<T> Method()
        {
            var newList = new List<T>();
            return newList;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void ElementaryMethodsOfTypeInCollectionNotOverridden_GenericTypeFromMethod()
        {
            var original = @"
using System.Collections.Generic;
namespace ConsoleApplication1
{
    internal class MyClass
    {
        public static List<T1> Method<T1>()
        {
            var newList = new List<T1>();
            return newList;
        }
    }
}";

            VerifyDiagnostic(original);
        }
    }
}