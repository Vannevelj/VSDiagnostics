using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.TryCastWithoutUsingAsNotNull;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class TryCastUsingAsNotNullInsteadOfIsAsTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new TryCastWithoutUsingAsNotNullAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new TryCastWithoutUsingAsNotNullCodeFix();

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndReferenceType()
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
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndValueType()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndObjectIsUsedBeforeIs()
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
            object o = ""sample"";
            Console.Write(o.GetType());
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            Console.Write(o.GetType());
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndObjectIsMethodParameter()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_AndElseClause()
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
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
            else
            {
                Console.Write(""something"");
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_MultipleIrrelevantCasts()
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
            object o = 5;
            if (o is int)
            {
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
                var oAsInt = o as int?;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                object irrelevant = 10.0;
                var irrelevantAsDouble = irrelevant as double?;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_ReferenceType()
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
            object o = ""test"";
            if (o is string)
            {
                var oAsString = (string) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""test"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_Struct()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = (int) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_NullableStruct()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = (int?) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_WithoutCorrespondingCast()
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
            object o = 5;
            object irrelevant = 10.0;
            if (o is int)
            {
                var irrelevantAsDouble = irrelevant as double?;
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_TryCastNullCheck()
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
            object o = 5;
            int? oAsInt = o as int?;
            if (oAsInt != null)
            {

            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_MultipleRelevantCasts()
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
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string;
                string anotherString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, 
                string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"),
                string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_MultipleIfConditions()
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
            object o = ""sample"";
            if (o is string && 1 == 1)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null && 1 == 1)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ReferencingParameter()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                string oAsString = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_AsCastInIfCondition()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string && (o as string).Length > 1)
            {
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null && (oAsString).Length > 1)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCastInIfCondition()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string && ((string) o).Length > 1)
            {
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null && (oAsString).Length > 1)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastInSeparateExpression_ReferenceType()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is string)
            {
                bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(o as string);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsString = o as string;
            if (oAsString != null)
            {
                bool contains = new[] { ""test"", ""test"", ""test"" }.Contains(oAsString);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastInSeparateExpression_ValueType()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                bool contains = new[] { 5, 6, 7 }.Contains((o as int?).Value);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                bool contains = new[] { 5, 6, 7 }.Contains((oAsInt32).Value);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastInSeparateExpression_ValueType_NullableCollectionElements()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                bool contains = new int?[] { 5, 6, 7 }.Contains(o as int?);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                bool contains = new int?[] { 5, 6, 7 }.Contains(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastToSelfdefinedType()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is Other)
            {
                Other myVar = o as Other;
            }
        }
    }

    class Other { }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsOther = o as Other;
            if (oAsOther != null)
            {
            }
        }
    }

    class Other { }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_CastUsingMethodReference()
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
            if (Get() is string)
            {
                string myVar = Get() as string;
            }
        }

        object Get()
        {
            return null;
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_SplitVariableDefinitionAndAssignment()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            string myVar = null;
            if (o is string)
            {
                myVar = o as string;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            string myVar = null;
            var oAsString = o as string;
            if (oAsString != null)
            {
                myVar = oAsString;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DifferentTypes()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                var x = o as string;
            }
        }
    }
}";
            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IsAs_UsagesOfCastedVariable()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = o as int?;

                Console.Write(oAsInt);
                Console.Write(oAsInt);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                Console.Write(oAsInt32);
                Console.Write(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_UsagesOfCastedVariable()
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
            object o = 5;
            if (o is int)
            {
                var oAsInt = (int) o;

                Console.Write(oAsInt);
                Console.Write(oAsInt);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                Console.Write(oAsInt32);
                Console.Write(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        /// <summary>
        ///     Known issue, see issue https://github.com/Vannevelj/VSDiagnostics/issues/379 for more info
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TryCastWithoutUsingAsNotNull_AnonymousTypeRenaming()
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
            object o = 5;
            if (o is int)
            {
                var x = (int) o;
                var y = new { x };
                var z = y.x;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = 5;
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                var y = new { oAsInt32 };
                var z = y.oAsInt32;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }


        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ConflictingLocalReferringToField()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            if (o is int)
            {
                var someVar = (int)o;

                Console.Write(someVar);
                Console.Write(oAsInt32);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            var oAsInt32_1 = o as int?;
            if (oAsInt32_1 != null)
            {
                Console.Write(oAsInt32_1);
                Console.Write(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ConflictingLocalExplicitlyReferringToField()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            if (o is int)
            {
                var someVar = (int)o;

                Console.Write(someVar);
                Console.Write(this.oAsInt32);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            var oAsInt32_1 = o as int?;
            if (oAsInt32_1 != null)
            {
                Console.Write(oAsInt32_1);
                Console.Write(this.oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_BranchedIfStatement()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                var someVar = (int)o;
            }
            else if (o is string)
            {
                var someVar = (string)o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            var oAsString = o as string;
            if (oAsInt32 != null)
            {
            }
            else if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"),
                string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_ValueType_SeparateExpression()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is double && (new double[] { 5.0, 6.0, 7.0 }.Contains((double) o)))
            {
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsDouble = o as double?;
            if (oAsDouble != null && (new double[] { 5.0, 6.0, 7.0 }.Contains(oAsDouble.Value)))
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_BranchedIfStatement_ConflictingField_CastInExpression()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            if (o is int)
            {
                var someVar = (int) o;
                Console.WriteLine(oAsInt32);
            }
            else if (o is string)
            {
                var someVar = (string) o;
            } 
            else if (o is double && (new double[] { 5.0, 6.0, 7.0 }.Contains((double) o)))
            {
                Console.WriteLine(oAsInt32);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            var oAsInt32_1 = o as int?;
            var oAsString = o as string;
            var oAsDouble = o as double?;
            if (oAsInt32_1 != null)
            {
                Console.WriteLine(oAsInt32);
            }
            else if (oAsString != null)
            {
            }
            else if (oAsDouble != null && (new double[] { 5.0, 6.0, 7.0 }.Contains(oAsDouble.Value)))
            {
                Console.WriteLine(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"),
                string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"),
                string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_DirectCast_ValueType_SeparateExpression_NullableCollection()
        {
            var original = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is double && (new double?[] { 5.0, 6.0, 7.0 }.Contains((double) o)))
            {
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;
using System.Linq;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsDouble = o as double?;
            if (oAsDouble != null && (new double?[] { 5.0, 6.0, 7.0 }.Contains(oAsDouble.Value)))
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ConflictingLocal_InSeparateBranch()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            if (o is int)
            {
                Console.Write((int) o);
            } else {
                Console.Write(oAsInt32);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            var oAsInt32_1 = o as int?;
            if (oAsInt32_1 != null)
            {
                Console.Write(oAsInt32_1.Value);
            } else {
                Console.Write(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ConflictingLocal_InSeparateBranch_WithExplicitThis()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            if (o is int)
            {
                Console.Write((int) o);
            } else {
                Console.Write(this.oAsInt32);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            var oAsInt32_1 = o as int?;
            if (oAsInt32_1 != null)
            {
                Console.Write(oAsInt32_1.Value);
            } else {
                Console.Write(this.oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ConflictingName_1()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            if (o is int)
            {
                Console.Write((int) o);
                Console.Write(oAsInt32);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32;

        void Method(object o)
        {
            var oAsInt32_1 = o as int?;
            if (oAsInt32_1 != null)
            {
                Console.Write(oAsInt32_1.Value);
                Console.Write(oAsInt32);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ConflictingName_2()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32_1;

        void Method(object o)
        {
            if (o is int)
            {
                Console.Write((int) o);
                Console.Write(oAsInt32_1);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32_1;

        void Method(object o)
        {
            var oAsInt32_2 = o as int?;
            if (oAsInt32_2 != null)
            {
                Console.Write(oAsInt32_2.Value);
                Console.Write(oAsInt32_1);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_ConflictingName_3()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32_10;

        void Method(object o)
        {
            if (o is int)
            {
                Console.Write((int) o);
                Console.Write(oAsInt32_10);
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        private int oAsInt32_10;

        void Method(object o)
        {
            var oAsInt32_11 = o as int?;
            if (oAsInt32_11 != null)
            {
                Console.Write(oAsInt32_11.Value);
                Console.Write(oAsInt32_10);
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_TernaryOperator()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var x = o is int ? 5 : 6;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_TernaryOperator_WithCasts()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var x = o is int ? ((int) o) : ((double) o);
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_Return()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        object Method(object o)
        {
            return o is int ? 5 : 6;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_Return_WithCasts()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        object Method(object o)
        {
            return o is int ? ((int) o) : ((double) o);
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_Return_IsStatement()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        bool Method(object o)
        {
            return o is int;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_Local()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var isInt = o is int;
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IrrelevantIfParent()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (true)
            {
                var res = o is int ? ((int)o) : ((double)o);
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IrrelevantIfParent_2()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (true)
            {
                if (true)
                {
                    var res = o is int ? ((int)o) : ((double)o);
                }
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_IrrelevantIfAncestor()
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
            object o = ""sample"";
            if (true)
            {
                if (o is string)
                {
                    string oAsString = o as string;
                }
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            if (true)
            {
                var oAsString = o as string;
                if (oAsString != null)
                {
                }
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_Trivia()
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
            object o = ""sample"";
            if (o is string)
            {
                // Test
                string oAsString = o as string; // Test
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_MultipleDeclarators()
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
            object o = ""sample"";
            if (o is string)
            {
                string oAsString = o as string, s = ""test"";
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method()
        {
            object o = ""sample"";
            var oAsString = o as string;
            if (oAsString != null)
            {
                string s = ""test"";
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_TernaryWithAppropriateIfCondition()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int x, y;

        void Method(object o)
        {
            if (o is int)
            {
                var res = x > y ? (int) o : (double) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        int x, y;

        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                var res = x > y ? oAsInt32.Value : (double) o;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_TernaryWithAppropriateIfCondition_RepeatedCheck()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                var res = o is int ? (int) o : (double) o;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                var res = o is int ? oAsInt32.Value : (double) o;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_RedundantCheck()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (o is int)
            {
                var x = (int) o;
                var y = o is int;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (oAsInt32 != null)
            {
                var y = o is int;
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_While()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (true)
            {
                while(o is int)
                {
                    var x = o as int?;
                    o = ""test"";
                }
            }
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void TryCastWithoutUsingAsNotNull_NegativeCondition()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            if (!(o is int))
            {
                var x = o as int?;
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    class MyClass
    {
        void Method(object o)
        {
            var oAsInt32 = o as int?;
            if (!(oAsInt32 != null))
            {
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(TryCastWithoutUsingAsNotNullAnalyzer.Rule.MessageFormat.ToString(), "o"));
            VerifyFix(original, expected);
        }
    }
}