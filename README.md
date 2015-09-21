# VSDiagnostics
A collection of code-quality analyzers based on the new Roslyn platform. This project aims to ensure code-quality as you type it in your editor rather than having to do this as a separate build-step. Likewise it also tries to help avoid some common pitfalls.

<img src="https://cloud.githubusercontent.com/assets/2777107/7789534/a06db792-0264-11e5-955f-11cbf3261d4f.gif" />

[Get it from NuGet!](https://www.nuget.org/packages/VSDiagnostics/)

[![Build status](https://ci.appveyor.com/api/projects/status/c5f15ckfb5wv91ma?svg=true)](https://ci.appveyor.com/project/Vannevelj/vsdiagnostics)
[![Test status](http://teststatusbadge.azurewebsites.net/api/status/Vannevelj/vsdiagnostics)](https://ci.appveyor.com/project/Vannevelj/vsdiagnostics)

Keep in mind that this project is under active development. If you encounter bugs, let us know!

## What is an analyzer exactly?

> With the release of Visual Studio 2015 RC, we also received the pretty much final implementation of the Diagnostics implementation. This SDK allows us to create our own diagnostics to help us write proper code that’s being verified against those rules in real-time: you don’t have to perform the verification at a separate build-step. What’s more is that we can combine that with a code fix: a shortcut integrated in Visual Studio that provides us a solution to what we determine to be a problem.

> This might sound a little abstract but if you’ve been using Visual Studio (and/or Resharper) then you know what I mean: have you ever written your classname as class rather than Class? This is a violation of the C# naming conventions and visual studio will warn you about it and provide you a quickfix to turn it into a proper capitalized word. This is exactly the behaviour we can create and which is integrated seemlessly in Visual Studio.

Full text available on [my blog](http://www.vannevel.net/2015/05/03/getting-started-with-your-first-diagnostic/).

## What is available?

Currently these diagnostics are implemented:

| Category | Name | Description
|:-:|:-:|:-:
| Async | AsyncMethodWithoutAsyncSuffix | Asynchronous methods should end with -Async.
| Attributes | AttributeWithEmptyArgumentList | Attributes with empty argument lists can have the argument list removed.
| Attributes | EnumCanHaveFlagsAttribute | Gives an enum the [Flags] attribute.
| Attributes | ObsoleteAttributeWithoutReason | Complains if the [Obsolete] attribute is used without an explicit reason.
| Attributes | OnPropertyChangedWithoutCallerMemberName | The `OnPropertyChanged()` method can automatically get the caller member name.
| Exceptions | ArgumentExceptionWithoutNameofOperator | `ArgumentException` and its subclasses should use `nameof()` when they refer to a method parameter.
| Exceptions | CatchNullReferenceException  | Guards against catching a NullReferenceException.
| Exceptions | EmptyArgumentException | Guards against using an `ArgumentException` without specifying which argument.
| Exceptions | EmptyCatchClause | Warns when an exception catch block is empty.
| Exceptions | RethrowExceptionWithoutLosingStacktrace | Warns when an exception is rethrown in a way that it loses the stacktrace.
| Exceptions | SingleGeneralException  | Guards against using a catch-all clause.
| General | AsToCast | Allows you to change as statements to cast statements.
| General | CastToAs | Allows you to change cast statements to as statements.
| General | CompareBooleanToFalseLiteral | A boolean expression doesn't have to be compared to `false`.
| General | CompareBooleanToTrueLiteral | A boolean expression doesn't have to be compared to `true`.
| General | ConditionalOperatorReturnsDefaultOptions | The conditional operator shouldn't return redundant `true` and `false` literals.
| General | ConditionalOperatorReturnsInvertedDefaultOptions | The conditional operator shouldn't return redundant `false` and `true` literals.
| General | ConditionIsAlwaysFalse | Complains about `if` statements of the form `if (statement) { /* body */ }`, where "statement" is always evaluates to `false`.
| General | ConditionIsAlwaysTrue | Complains about `if` statements of the form `if (statement) { /* body */ }`, where "statement" is always evaluates to `true`.
| General | ExplicitAccessModifiers | Inserts the default access modifier for a declaration.
| General | GotoDetection | Detects usage of the `goto` keyword.
| General | IfStatementWithoutBraces | Changes one-liner `if` and `else` statements to be surrounded in a block.
| General | NamingConventions | Implements the most common configuration of naming conventions.
| General | NonEncapsulatedOrMutableField | A `public`, `internal` or `protected internal` non-`const`, non-`readonly` field should be used as a property.
| General | NullableToShorthand | Changes `Nullable<T>` to `T?`.
| General | OnPropertyChangedWithoutNameOfOperator | Use the `nameof()` operator in conjunction with `OnPropertyChanged`.
| General | SimplifyExpressionBodiedMember | Simplify the expression using an expression-bodied member.
| General | SingleEmptyConstructor | Warns about using a redundant default constructor.
| General | TryCastWithoutUsingAsNotNull | A conversion can be done using `as` + a `null` comparison.
| General | TypeToVar | Use `var` instead of an explicit type.
| General | UseAliasesInsteadOfConcreteType | Use the built-in type aliases instead the concrete type.
| Strings | ReplaceEmptyStringWithStringDotEmpty | Use `string.Empty` instead of `""`.
| Strings | StringPlaceholdersInWrongOrder | Adjusts the placeholders in `string.Format()` calls to be in numerical order.
| Structs | StructShouldNotMutateSelf | Warns when a struct attempts to assign 'this' to a new instance of the struct.
| Tests | RemoveTestSuffix | Test methods do not need to use the "Test" suffic.
| Tests | TestMethodWithoutPublicModifier | Change the access modifier to `public` for all methods annotated as test. Supports NUnit, MSTest and xUnit.net.

## How do I use this?

Simply head over to [NuGet](https://www.nuget.org/packages/VSDiagnostics/) and install it! If you don't immediately find it: make sure you're also looking through the NuGet V2 package source.

## Can I request diagnostics?

Yes, you can! Create an issue and we'll take a look at your proposal. 

## What if I don't like a diagnostic?

Every diagnostic can be turned off for a single line or for the entire project. At most, you will have to ignore the diagnostic once. For this reason every diagnostic is turned on by default.

## Can I contribute?

Definitely! Take a look at the open issues and see if there's anything that interests you. Submit your pull request and we'll take a look at it as soon as possible.

If you think you're going to make larger changes than a single implementation then we would ask you to get in contact with us first so we can discuss it and prevent unneeded work.

You'll need the [Visual Studio 2015 Release Candidate](https://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs.aspx) and [the SDK](https://www.microsoft.com/en-us/download/details.aspx?id=46850) to get started.
