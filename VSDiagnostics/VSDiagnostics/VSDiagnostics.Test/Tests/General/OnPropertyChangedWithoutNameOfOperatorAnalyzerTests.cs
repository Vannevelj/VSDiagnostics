using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.DiagnosticResults;
using RoslynTester.Helpers;
using VSDiagnostics.Diagnostics.General.OnPropertyChangedWithoutNameOfOperator;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class OnPropertyChangedWithoutNameOfOperatorAnalyzerTests : CodeFixVerifier
    {
        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithIdenticalString_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.ComponentModel;

namespace ConsoleApplication1
{
    class MyClass : INotifyPropertyChanged
    {
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(""IsEnabled"");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }                
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = OnPropertyChangedWithoutNameOfOperatorAnalyzer.DiagnosticId,
                Message = OnPropertyChangedWithoutNameOfOperatorAnalyzer.Message,
                Severity = OnPropertyChangedWithoutNameOfOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithDifferentlyCasedString_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.ComponentModel;

namespace ConsoleApplication1
{
    class MyClass : INotifyPropertyChanged
    {
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(""iSeNabled"");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }                
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = OnPropertyChangedWithoutNameOfOperatorAnalyzer.DiagnosticId,
                Message = OnPropertyChangedWithoutNameOfOperatorAnalyzer.Message,
                Severity = OnPropertyChangedWithoutNameOfOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithDifferentStringAndNoCorrespondingProperty_DoesNotInvokeWarning()
        {
            var original = @"
using System;
using System.Text;
using System.ComponentModel;

namespace ConsoleApplication1
{
    class MyClass : INotifyPropertyChanged
    {
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(""SomethingElse"");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }                
    }
}";

            VerifyCSharpDiagnostic(original);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithDifferentStringAndCorrespondingProperty_InvokesWarning()
        {
            var original = @"
using System;
using System.Text;
using System.ComponentModel;

namespace ConsoleApplication1
{
    class MyClass : INotifyPropertyChanged
    {
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(""IsAnotherBoolean"");
            }
        }

        private bool _anotherBoolean;
        public bool IsAnotherBoolean
        {
            get { return _anotherBoolean; }
            set
            {
                _anotherBoolean = value;
                OnPropertyChanged(""IsAnotherBoolean"");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }                
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = OnPropertyChangedWithoutNameOfOperatorAnalyzer.DiagnosticId,
                Message = OnPropertyChangedWithoutNameOfOperatorAnalyzer.Message,
                Severity = OnPropertyChangedWithoutNameOfOperatorAnalyzer.Severity,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 12, 30)
                    }
            };

            VerifyCSharpDiagnostic(original, expectedDiagnostic);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new OnPropertyChangedWithoutNameOfOperatorAnalyzer();
        }
    }
}