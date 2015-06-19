using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.OnPropertyChangedWithoutNameOfOperator;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class OnPropertyChangedWithoutNameOfOperatorAnalyzerTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new OnPropertyChangedWithoutNameOfOperatorAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new OnPropertyChangedWithoutNameOfOperatorCodeFix();

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

            var expected = @"
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
                OnPropertyChanged(nameof(IsEnabled));
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

            VerifyDiagnostic(original, string.Format(OnPropertyChangedWithoutNameOfOperatorAnalyzer.Rule.MessageFormat.ToString(), "IsEnabled"));
            VerifyFix(original, expected);
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

            var expected = @"
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
                OnPropertyChanged(nameof(IsEnabled));
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

            VerifyDiagnostic(original, string.Format(OnPropertyChangedWithoutNameOfOperatorAnalyzer.Rule.MessageFormat.ToString(), "IsEnabled"));
            VerifyFix(original, expected);
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

            VerifyDiagnostic(original);
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
                OnPropertyChanged(nameof(IsAnotherBoolean));
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

            var expected = @"
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
                OnPropertyChanged(nameof(IsAnotherBoolean));
            }
        }

        private bool _anotherBoolean;
        public bool IsAnotherBoolean
        {
            get { return _anotherBoolean; }
            set
            {
                _anotherBoolean = value;
                OnPropertyChanged(nameof(IsAnotherBoolean));
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

            VerifyDiagnostic(original, string.Format(OnPropertyChangedWithoutNameOfOperatorAnalyzer.Rule.MessageFormat.ToString(), "IsAnotherBoolean"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithNameOfOperator_DoesNotInvokeWarning()
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
                OnPropertyChanged(nameof(IsEnabled));
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

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithMultipleArguments_InvokesWarning()
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
                OnPropertyChanged(""IsEnabled"", true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName, bool someBoolean)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }                
    }
}";

            var expected = @"
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
                OnPropertyChanged(nameof(IsEnabled), true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName, bool someBoolean)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }                
    }
}";

            VerifyDiagnostic(original, string.Format(OnPropertyChangedWithoutNameOfOperatorAnalyzer.Rule.MessageFormat.ToString(), "IsEnabled"));
            VerifyFix(original, expected);
        }
    }
}