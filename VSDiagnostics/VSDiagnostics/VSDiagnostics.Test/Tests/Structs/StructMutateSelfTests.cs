using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynTester.Helpers.CSharp;
using VSDiagnostics.Diagnostics.Structs;
using VSDiagnostics.Diagnostics.Structs.StructShouldNotMutateSelf;

namespace VSDiagnostics.Test.Tests.Structs
{
    [TestClass]
    public class StructMutateSelfTest : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new StructShouldNotMutateSelfAnalyzer();

        [TestMethod]
        public void StructShouldNotMutateSelf_StructThatMutatesSelf()
        {
            var original = @"
struct X
{
	private int x;

	public X(int value)
	{
		x = value;
	}

	public void Mutate()
	{
		this = new X(5);
	}
}";

            VerifyDiagnostic(original, string.Format(StructShouldNotMutateSelfAnalyzer.Rule.MessageFormat.ToString()));
        }

        [TestMethod]
        public void StructShouldNotMutateSelf_AssigningInPropertySetter()
        {
            var original = @"
struct Rectangle
{
	private int _width;
    private int _height;

	public Rectangle(int width, int height)
	{
		_width = width;
        _height = height;
	}

	public int Width { 
        get { return _width; }
        set { this = new Rectangle(value, _height); }
    }
}";

            VerifyDiagnostic(original, string.Format(StructShouldNotMutateSelfAnalyzer.Rule.MessageFormat.ToString()));
        }
 
        [TestMethod]
        public void StructShouldNotMutateSelf_AssigningToAVariableVar()
        {
            var original = @"
struct X
{
	private int x;

	public X(int value)
	{
		x = value;
	}

	public void NoMutate()
	{
		var x2 = new X(5);
	}
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StructShouldNotMutateSelf_AssigningToAVariable()
        {
            var original = @"
struct X
{
	private int x;

	public X(int value)
	{
		x = value;
	}

	public void NoMutate()
	{
		X x2 = new X(5);
	}
}";

            VerifyDiagnostic(original);
        }

        [TestMethod]
        public void StructShouldNotMutateSelf_AssigningToAVariableObject()
        {
            var original = @"
struct X
{
	private int x;

	public X(int value)
	{
		x = value;
	}

	public void NoMutate()
	{
		object x2 = new X(5);
	}
}";

            VerifyDiagnostic(original);
        }
    }
}