using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Attributes.OnPropertyChangedWithoutCallerMemberName;

namespace VSDiagnostics.Test.Tests.Attributes
{
    [TestClass]
    public class OnPropertyChangedWithoutCallerMemberNameTests : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new OnPropertyChangedWithoutCallerMemberNameAnalyzer();

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
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
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
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
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
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            VerifyDiagnostic(original, OnPropertyChangedWithoutCallerMemberNameAnalyzer.Rule.MessageFormat.ToString());
        }
    }
}