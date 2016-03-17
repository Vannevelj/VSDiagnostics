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
            TestOutput(NamingConventionExtensions.Lower, "abcd", "abcd");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.Lower, "Abcd", "abcd");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.Lower, "_abcd", "abcd");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_Short()
        {
            TestOutput(NamingConventionExtensions.Lower, "a", "a");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.Lower, "_a", "a");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.Lower, "a_", "a");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.Lower, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_LowerCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.Lower, "FFFrankWhatDidYouDo", "fffrankWhatDidYouDo");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_LowerCase()
        {
            TestOutput(NamingConventionExtensions.Upper, "abcd", "Abcd");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.Upper, "Abcd", "Abcd");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.Upper, "_abcd", "Abcd");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_Short()
        {
            TestOutput(NamingConventionExtensions.Upper, "a", "A");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.Upper, "_a", "A");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.Upper, "a_", "A");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.Upper, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_UpperCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.Upper, "ABCd", "Abcd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_LowerCase()
        {
            TestOutput(NamingConventionExtensions.IUpper, "abcd", "IAbcd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.IUpper, "Abcd", "IAbcd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.IUpper, "_abcd", "IAbcd");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortLower()
        {
            TestOutput(NamingConventionExtensions.IUpper, "a", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortUpper()
        {
            TestOutput(NamingConventionExtensions.IUpper, "A", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.IUpper, "_a", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.IUpper, "a_", "IA");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.IUpper, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_InterfacePrefixUpperCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.IUpper, "ABCd", "IAbcd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_LowerCase()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "abcd", "_abcd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_LeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "Abcd", "_abcd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_LeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "_abcd", "_abcd");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortLower()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "a", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortUpper()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "A", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortWithLeadingUnderscore()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "_a", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_ShortWithTrailingUnderscore()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "a_", "_a");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_OnlyUnderScores()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "____", "____");
        }

        [TestMethod]
        public void NamingConvention_UnderscoreLowerCamelCase_MultipleLeadingUpperCase()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "ABCd", "_abcd");
        }

        [TestMethod]
        public void GetNormalizedString_1()
        {
            TestOutput(NamingConventionExtensions.Lower, "_allo_ello", "alloEllo");
        }

        [TestMethod]
        public void GetNormalizedString_2()
        {
            TestOutput(NamingConventionExtensions.Upper, "_allo_ello", "AlloEllo");
        }

        [TestMethod]
        public void GetNormalizedString_3()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "_allo_ello", "_alloEllo");
        }

        [TestMethod]
        public void GetNormalizedString_4()
        {
            TestOutput(NamingConventionExtensions.Lower, "allo", "allo");
        }

        [TestMethod]
        public void GetNormalizedString_5()
        {
            TestOutput(NamingConventionExtensions.Upper, "allo", "Allo");
        }

        [TestMethod]
        public void GetNormalizedString_6()
        {
            TestOutput(NamingConventionExtensions.IUpper, "_allo_ello", "IAlloEllo");
        }

        [TestMethod]
        public void GetNormalizedString_7()
        {
            TestOutput(NamingConventionExtensions.IUpper, "ITest", "ITest");
        }

        [TestMethod]
        public void GetNormalizedString_8()
        {
            TestOutput(NamingConventionExtensions.IUpper, "Itest", "ITest");
        }

        [TestMethod]
        public void GetNormalizedString_9()
        {
            TestOutput(NamingConventionExtensions.IUpper, "ITEst", "ITest");
        }

        [TestMethod]
        public void GetNormalizedString_10()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "ITEst", "_itest");
        }

        [TestMethod]
        public void GetNormalizedString_11()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "hello_man", "_helloMan");
        }

        [TestMethod]
        public void GetNormalizedString_12()
        {
            TestOutput(NamingConventionExtensions.UnderscoreLower, "MY_SNAKE_CASE", "_mySnakeCase");
        }

        [TestMethod]
        public void GetNormalizedString_13()
        {
            TestOutput(NamingConventionExtensions.Lower, "MY_SNAKE_CASE", "mySnakeCase");
        }

        [TestMethod]
        public void GetNormalizedString_14()
        {
            TestOutput(NamingConventionExtensions.Upper, "MY_SNAKE_CASE", "MySnakeCase");
        }

        [TestMethod]
        public void GetNormalizedString_15()
        {
            TestOutput(NamingConventionExtensions.Upper, "MyVariable", "MyVariable");
        }

        [TestMethod]
        public void GetNormalizedString_16()
        {
            TestOutput(NamingConventionExtensions.Lower, "MyVariable2", "myVariable2");
        }

        [TestMethod]
        public void GetNormalizedString_17()
        {
            TestOutput(NamingConventionExtensions.IUpper, "IBufferMyBuffer", "IBufferMyBuffer");
        }

        private void TestOutput(Func<string, string> func, string input, string expected)
        {
            var result = NamingConventionExtensions.GetNormalizedString(input, func);
            Assert.AreEqual(expected, result);
        }
    }
}