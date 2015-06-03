using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSDiagnostics.Diagnostics.General.NamingConventions;

namespace VSDiagnostics.Test.Tests.Utilities
{
    [TestClass]
    public class NamingConventionsTests
    {
        [TestMethod]
        public void NamingConvention_LowerCamelCase_LowerCase()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "abcd", "abcd");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "Abcd", "abcd");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "_abcd", "abcd");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_Short()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "a", "a");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "_a", "a");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "a_", "a");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetLowerCamelCaseIdentifier, "ABCd", "aBCd");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_LowerCase()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "abcd", "Abcd");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "Abcd", "Abcd");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "_abcd", "Abcd");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_Short()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "a", "A");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "_a", "A");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "a_", "A");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetUpperCamelCaseIdentifier, "ABCd", "ABCd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_LowerCase()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "abcd", "IAbcd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "Abcd", "IAbcd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "_abcd", "IAbcd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortLower()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "a", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortUpper()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "A", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "_a", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "a_", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetInterfacePrefixUpperCamelCaseIdentifier, "ABCd", "IABCd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_LowerCase()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "abcd", "_abcd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "Abcd", "_abcd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "_abcd", "_abcd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortLower()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "a", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortUpper()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "A", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "_a", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "a_", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.GetUnderscoreLowerCamelCaseIdentifier, "ABCd", "_aBCd");
        }

        private void TestOutput(Func<string, string> func, string input, string expected)
        {
            Assert.AreEqual(expected, func(input));
        }
    }
}