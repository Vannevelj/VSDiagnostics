using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.OnPropertyChangedWithoutCallerMemberName;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class OnPropertyChangedWithoutCallerMemberNameTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new OnPropertyChangedWithoutCallerMemberNameAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new OnPropertyChangedWithoutCallerMemberNameCodeFix();

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChanged_InvokesWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var result = @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassDoesNotImplementINotifyPropertyChanged_DoesNotInvokeWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    class Foo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChangedAndCustomInterface_InvokesWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    interface IFoo
    {
    }

    class Foo : INotifyPropertyChanged, IFoo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = ""Biz"")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var result = @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1
{
    interface IFoo
    {
    }

    class Foo : INotifyPropertyChanged, IFoo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = ""Biz"")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsCustomInterfaceAndINotifyPropertyChanged_InvokesWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    interface IFoo
    {
    }

    class Foo : IFoo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var result = @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1
{
    interface IFoo
    {
    }

    class Foo : IFoo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChanged_MultipleMethods_InvokesWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        void PropertyChanged(string foo) { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var result = @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        void PropertyChanged(string foo) { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChanged_MultipleParams_DoesNotInvokeWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName, int foo)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChanged_OneParamTypeNotString_DoesNotInvokeWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(int foo)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChanged_ParamAlreadyHasAttribute_DoesNotInvokeWarning()
        {
            var original = @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChanged_ParamAlreadyHasDifferentAttribute_InvokesWarning()
        {
            var original = @"
using System;
using System.ComponentModel;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([Obsolete] string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var result = @"
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([Obsolete][CallerMemberName] string propertyName = """")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutCallerMemberName_ClassImplementsINotifyPropertyChanged_RemovesReferenceParam_InvokesWarning()
        {
            var original = @"
using System.ComponentModel;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        private int _bar = 0;
        public int Bar {
            get { return _bar; }
            set
            {
                _bar = value;
                OnPropertyChanged(""Bar""); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var result = @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConsoleApplication1
{
    class Foo : INotifyPropertyChanged
    {
        private int _bar = 0;
        public int Bar {
            get { return _bar; }
            set
            {
                _bar = value;
                OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
            VerifyFix(original, result);
        }
    }
}