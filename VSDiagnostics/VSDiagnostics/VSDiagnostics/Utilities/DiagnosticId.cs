namespace VSDiagnostics.Utilities
{
    public static class DiagnosticId
    {
        public const string AsyncMethodWithoutAsyncSuffix = "VSD0001";
        public const string AttributeWithEmptyArgumentList = "VSD0002";
        public const string EnumCanHaveFlagsAttribute = "VSD0003";
        public const string FlagsEnumValuesAreNotPowersOfTwo = "VSD0004";
        public const string ObsoleteAttributeWithoutReason = "VSD0005";
        public const string OnPropertyChangedWithoutCallerMemberName = "VSD0006";
        public const string ArgumentExceptionWithoutNameofOperator = "VSD0007";
        public const string CatchingNullReferenceException = "VSD0008";
        public const string EmptyArgumentException = "VSD0009";
        public const string EmptyCatchClause = "VSD0010";
        public const string RethrowExceptionWithoutLosingStacktrace = "VSD0011";
        public const string SingleGeneralException = "VSD0012";
        public const string AsToCast = "VSD0013";
        public const string CastToAs = "VSD0014";
        public const string CompareBooleanToFalseLiteral = "VSD0015";
        public const string CompareBooleanToTrueLiteral = "VSD0016";
        public const string ConditionalOperatorReturnsDefaultOptions = "VSD0017";
        public const string ConditionalOperatorReturnsInvertedDefaultOptions = "VSD0018";
        public const string ConditionIsConstant = "VSD0020";
        public const string ExplicitAccessModifiers = "VSD0021";
        public const string GotoDetection = "VSD0022";
        public const string IfStatementWithoutBraces = "VSD0023";
        public const string LoopStatementWithoutBraces = "VSD0024";
        public const string NamingConventions = "VSD0025";
        public const string NonEncapsulatedOrMutableField = "VSD0026";
        public const string NullableToShorthand = "VSD0027";
        public const string OnPropertyChangedWithoutNameofOperator = "VSD0028";
        public const string SimplifyExpressionBodiedMember = "VSD0029";
        public const string SingleEmptyConstructor = "VSD0030";
        public const string TryCastWithoutUsingAsNotNull = "VSD0031";
        public const string TypeToVar = "VSD0032";
        public const string UseAliasesInsteadOfConcreteType = "VSD0033";
        public const string ReplaceEmptyStringWithStringDotEmpty = "VSD0034";
        public const string StringPlaceholdersInWrongOrder = "VSD0035";
        public const string StructShouldNotMutateSelf = "VSD0036";
        public const string RemoveTestSuffix = "VSD0037";
        public const string TestMethodWithoutPublicModifier = "VSD0038";
        public const string FlagsEnumValuesDontFit = "VSD0039";
        public const string NamingConventionsConflictingMember = "VSD0040"; // Unused. What to do?
        public const string SyncMethodWithAsyncSuffix = "VSD0041";
        public const string StringDotFormatWithDifferentAmountOfArguments = "VSD0042";
        public const string LoopedRandomInstantiation = "VSD0043";
        public const string SwitchDoesNotHandleAllEnumOptions = "VSD0044";
        public const string DivideIntegerByInteger = "VSD0045";
        public const string EqualsAndGetHashcodeNotImplementedTogether = "VSD0046";
        public const string ElementaryMethodsOfTypeInCollectionNotOverridden = "VSD0047";
        public const string RedundantPrivateSetter = "VSD0048";
        public const string SwitchIsMissingDefaultLabel = "VSD0049";
        public const string StructWithoutElementaryMethodsOverridden = "VSD0050";
        public const string UsingStatementWithoutBraces = "VSD0051";
        public const string ExceptionThrownFromImplicitOperator = "VSD0052";
        public const string ExceptionThrownFromPropertyGetter = "VSD0053";
        public const string ExceptionThrownFromStaticConstructor = "VSD0054";
        public const string ExceptionThrownFromFinallyBlock = "VSD0055";
        public const string ExceptionThrownFromEqualityOperator = "VSD0056";
        public const string ExceptionThrownFromDispose = "VSD0057";
        public const string ExceptionThrownFromFinalizer = "VSD0058";
        public const string ExceptionThrownFromGetHashCode = "VSD0059";
        public const string ExceptionThrownFromEquals = "VSD0060";
    }
}