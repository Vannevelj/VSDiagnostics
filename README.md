# VSDiagnostics
A collection of code-quality analyzers based on the new Roslyn platform. This project aims to ensure code-quality as you type it in your editor rather than having to do this as a separate build-step. Likewise it also tries to help avoid some common pitfalls.

<img src="https://cloud.githubusercontent.com/assets/2777107/12633986/2e05fc66-c576-11e5-92a2-3c192f2f0d89.gif" />

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
| Arithmetic | VSD0045 | The operands of a divisive expression are both integers and result in an implicit rounding.
| Async | VSD0001 | Asynchronous methods should end with -Async.
| Async | VSD0041 | A non-async, non-Task method should not end with -Async.
| Attributes | VSD0002 | Attributes with empty argument lists can have the argument list removed.
| Attributes | VSD0003 | Gives an enum the [Flags] attribute.
| Attributes | VSD0004 | A `[Flags]` enum its values are not explicit powers of 2
| Attributes | VSD0039 | A `[Flags]` enum its values are not explicit powers of 2 and does not fit in the specified enum type.
| Attributes | VSD0005 | Complains if the [Obsolete] attribute is used without an explicit reason.
| Attributes | VSD0006 | The `OnPropertyChanged()` method can automatically get the caller member name.
| Exceptions | VSD0007 | `ArgumentException` and its subclasses should use `nameof()` when they refer to a method parameter.
| Exceptions | VSD0008 | Guards against catching a NullReferenceException.
| Exceptions | VSD0009 | Guards against using an `ArgumentException` without specifying which argument.
| Exceptions | VSD0010 | Warns when an exception catch block is empty.
| Exceptions | VSD0011 | Warns when an exception is rethrown in a way that it loses the stacktrace.
| Exceptions | VSD0012 | Guards against using a catch-all clause.
| General | VSD0013 | Allows you to change as statements to cast statements.
| General | VSD0014 | Allows you to change cast statements to as statements.
| General | VSD0015 | A boolean expression doesn't have to be compared to `false`.
| General | VSD0016 | A boolean expression doesn't have to be compared to `true`.
| General | VSD0017 | The conditional operator shouldn't return redundant `true` and `false` literals.
| General | VSD0018 | The conditional operator shouldn't return redundant `false` and `true` literals.
| General | VSD0019 | Complains about `if` statements of the form `if (statement) { /* body */ }`, where "statement" always evaluates to `false`.
| General | VSD0020 | Complains about `if` statements of the form `if (statement) { /* body */ }`, where "statement" always evaluates to `true`.
| General | VSD0021 | Inserts the default access modifier for a declaration.
| General | VSD0022 | Detects usage of the `goto` keyword.
| General | VSD0023 | Changes one-liner `if` and `else` statements to be surrounded in a block.
| General | VSD0024 | Loop blocks should use braces to denote start and end.
| General | VSD0025 | Implements the most common configuration of naming conventions.
| General | VSD0026 | A `public`, `internal` or `protected internal` non-`const`, non-`readonly` field should be used as a property.
| General | VSD0027 | Changes `Nullable<T>` to `T?`.
| General | VSD0028 | Use the `nameof()` operator in conjunction with `OnPropertyChanged`.
| General | VSD0029 | Simplify the expression using an expression-bodied member.
| General | VSD0030 | Warns about using a redundant default constructor.
| General | VSD0031 | A conversion can be done using `as` + a `null` comparison.
| General | VSD0032 | Use `var` instead of an explicit type.
| General | VSD0033 | Use the built-in type aliases instead of the concrete type.
| General | VSD0044 | Missing enum member(s) in switched cases.
| General | VSD0046 | Equals() and GetHashcode() must be implemented together.
| Strings | VSD0034 | Use `string.Empty` instead of `""`.
| Strings | VSD0035 | Adjusts the placeholders in `string.Format()` calls to be in numerical order.
| Strings | VSD0042 | A `string.Format()` call lacks arguments and will cause a runtime exception
| Structs | VSD0036 | Warns when a struct attempts to assign 'this' to a new instance of the struct.
| Tests | VSD0037 | Test methods do not need to use the "Test" suffic.
| Tests | VSD0038 | Change the access modifier to `public` for all methods annotated as test. Supports NUnit, MSTest and xUnit.net.

## How do I use this?

Simply head over to [NuGet](https://www.nuget.org/packages/VSDiagnostics/) and install it! If you don't immediately find it: make sure you're also looking through the NuGet V2 package source.

## Can I request diagnostics?

Yes, you can! Create an issue and we'll take a look at your proposal. 

## What if I don't like a diagnostic?

Every diagnostic can be turned off for a single line or for the entire project. At most, you will have to ignore the diagnostic once. For this reason every diagnostic is turned on by default.
In order to do so, right click on 'Analyzers' under your references and select 'Open Active Rule Set'. This presents you with the following window to configure which diagnostics should be turned on and their default severity. If you wish to do so, you can change the severity to a more strict or more forgiving setting.

<img src="https://cloud.githubusercontent.com/assets/2777107/10696693/a1a2c82a-79a9-11e5-97bf-21f7e8c37356.PNG" />

## What's on the roadmap?

Most things are filed as an issue so if you want the complete picture -- head over there. However in broad terms:

 * Supporting more C# 6 features to ease the transition
 * Implement diagnostics aimed at removing unused code
 * Create a website that gives more detailed information
 * Add user-configurable settings for each diagnostic
 * Introduce a Visual Studio extension so it doesn't have to be tied to the project
 * Introduce a command-line tool so it isn't tied to a Visual Studio instance

## Can I contribute?

Definitely! Take a look at the open issues and see if there's anything that interests you. Submit your pull request and we'll take a look at it as soon as possible.

If you think you're going to make larger changes than a single implementation then we would ask you to get in contact with us first so we can discuss it and prevent unneeded work.

You'll need [Visual Studio 2015](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) and [the SDK](https://visualstudiogallery.msdn.microsoft.com/2ddb7240-5249-4c8c-969e-5d05823bcb89) to get started.

## Where can I find more information about every diagnostic?

Release 2.0.0 will come with a website where we document every diagnostic including reasoning behind its implementation choices. In the meantime you can take a look at [the tests](https://github.com/Vannevelj/VSDiagnostics/tree/develop/VSDiagnostics/VSDiagnostics/VSDiagnostics.Test/Tests) that belong to each diagnostic to see what exact scenarios we have accounted for.

## How can I get in contact?

You're always free to open an issue but if you would like something more direct you can drop by in [the StackExchange chat channel](http://chat.stackexchange.com/rooms/26639/vsdiagnostics) where the main contributors reside.
