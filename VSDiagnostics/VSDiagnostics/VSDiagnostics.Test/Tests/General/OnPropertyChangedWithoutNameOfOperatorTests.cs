using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.General.OnPropertyChangedWithoutNameOfOperator;

namespace VSDiagnostics.Test.Tests.General
{
    [TestClass]
    public class OnPropertyChangedWithoutNameOfOperatorTests : CSharpCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new OnPropertyChangedWithoutNameOfOperatorAnalyzer();

        protected override CodeFixProvider CodeFixProvider => new OnPropertyChangedWithoutNameOfOperatorCodeFix();

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithIdenticalString()
        {
            var original = @"
using System;
using System.Text;

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
        public void OnPropertyChangedWithoutNameOfOperator_WithDifferentlyCasedString()
        {
            var original = @"
using System;
using System.Text;

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
        public void OnPropertyChangedWithoutNameOfOperator_WithDifferentStringAndNoCorrespondingProperty()
        {
            var original = @"
using System;
using System.Text;

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
        public void OnPropertyChangedWithoutNameOfOperator_WithDifferentStringAndCorrespondingProperty()
        {
            var original = @"
using System;
using System.Text;

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
        public void OnPropertyChangedWithoutNameOfOperator_WithNameOfOperator()
        {
            var original = @"
using System;
using System.Text;

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
        public void OnPropertyChangedWithoutNameOfOperator_WithMultipleArguments()
        {
            var original = @"
using System;
using System.Text;

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

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_WithPartialClass()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    partial class MyClass : INotifyPropertyChanged
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
    }

    partial class MyClass
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    partial class MyClass : INotifyPropertyChanged
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
    }

    partial class MyClass
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
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
        public void OnPropertyChangedWithoutNameOfOperator_ParenthesizedExpression()
        {
            var original = @"
using System;
using System.Text;

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
                OnPropertyChanged((""IsEnabled""));
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
                OnPropertyChanged((nameof(IsEnabled)));
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
        public void OnPropertyChangedWithoutNameOfOperator_WithPartialClass_AndDifferentProperty()
        {
            var original = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    partial class MyClass : INotifyPropertyChanged
    {
	    private bool _isEnabled;
	    public bool IsEnabled
	    {
		    get { return _isEnabled; }
		    set
		    {
			    _isEnabled = value;
			    OnPropertyChanged(""OtherBoolean"");
            }
        }
    }

    partial class MyClass
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool OtherBoolean { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}";

            var expected = @"
using System;
using System.Text;

namespace ConsoleApplication1
{
    partial class MyClass : INotifyPropertyChanged
    {
	    private bool _isEnabled;
	    public bool IsEnabled
	    {
		    get { return _isEnabled; }
		    set
		    {
			    _isEnabled = value;
			    OnPropertyChanged(nameof(OtherBoolean)));
            }
        }
    }

    partial class MyClass
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool OtherBoolean { get; set; }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}";

            VerifyDiagnostic(original, string.Format(OnPropertyChangedWithoutNameOfOperatorAnalyzer.Rule.MessageFormat.ToString(), "OtherBoolean"));
            VerifyFix(original, expected);
        }

        [TestMethod]
        public void OnPropertyChangedWithoutNameOfOperator_ParenthesizedExpression_WithNameof()
        {
            var original = @"
using System;
using System.Text;

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
                OnPropertyChanged(((nameof(IsEnabled))));
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
    }
}