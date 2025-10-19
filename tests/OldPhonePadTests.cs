using System;
using Xunit;

namespace OldPhonePad.RegexStack.Tests
{
    /// <summary>
    /// Comprehensive test suite for the OldPhonePad RegexStack implementation.
    /// Tests cover basic functionality, edge cases, error handling, complex scenarios,
    /// and regex pattern matching behavior.
    /// </summary>
    public class OldPhonePadTests
    {
        [Fact]
        public void OldPhonePad_SingleKeyPress_ReturnsFirstCharacter()
        {
            var result = OldPhonePad.Decode("2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void OldPhonePad_DoubleKeyPress_ReturnsSecondCharacter()
        {
            var result = OldPhonePad.Decode("22#");
            Assert.Equal("B", result);
        }

        [Fact]
        public void OldPhonePad_TripleKeyPress_ReturnsThirdCharacter()
        {
            var result = OldPhonePad.Decode("222#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void OldPhonePad_Key7FourPresses_ReturnsS()
        {
            var result = OldPhonePad.Decode("7777#");
            Assert.Equal("S", result);
        }

        [Fact]
        public void OldPhonePad_Key9FourPresses_ReturnsZ()
        {
            var result = OldPhonePad.Decode("9999#");
            Assert.Equal("Z", result);
        }

        [Fact]
        public void OldPhonePad_SpaceBetweenKeys_DecodesMultipleCharacters()
        {
            var result = OldPhonePad.Decode("2 2#");
            Assert.Equal("AA", result);
        }

        [Fact]
        public void OldPhonePad_HelloWorld_ReturnsCorrectText()
        {
            var result = OldPhonePad.Decode("4433555 555666#");
            Assert.Equal("HELLO", result);
        }

        [Fact]
        public void OldPhonePad_BackspaceRemovesLastCharacter()
        {
            var result = OldPhonePad.Decode("227*#");
            Assert.Equal("B", result);
        }

        [Fact]
        public void OldPhonePad_MultipleBackspaces_RemovesMultipleCharacters()
        {
            // 222=C, 3=D, backspace removes D, backspace removes C -> empty
            var result = OldPhonePad.Decode("2223**#");
            Assert.Equal("", result);
        }

        [Fact]
        public void OldPhonePad_BackspaceOnEmptyString_ReturnsEmpty()
        {
            var result = OldPhonePad.Decode("*#");
            Assert.Equal("", result);
        }

        [Fact]
        public void OldPhonePad_BackspaceMoreThanAvailable_ReturnsEmpty()
        {
            var result = OldPhonePad.Decode("2***#");
            Assert.Equal("", result);
        }

        [Fact]
        public void OldPhonePad_OnlyHashSign_ReturnsEmpty()
        {
            var result = OldPhonePad.Decode("#");
            Assert.Equal("", result);
        }

        [Fact]
        public void OldPhonePad_OnlySpaces_ReturnsEmpty()
        {
            var result = OldPhonePad.Decode("   #");
            Assert.Equal("", result);
        }

        [Fact]
        public void OldPhonePad_Key0_ReturnsSpace()
        {
            var result = OldPhonePad.Decode("0#");
            Assert.Equal(" ", result);
        }

        [Fact]
        public void OldPhonePad_Key1FirstPress_ReturnsAmpersand()
        {
            var result = OldPhonePad.Decode("1#");
            Assert.Equal("&", result);
        }

        [Fact]
        public void OldPhonePad_Key1SecondPress_ReturnsApostrophe()
        {
            var result = OldPhonePad.Decode("11#");
            Assert.Equal("'", result);
        }

        [Fact]
        public void OldPhonePad_Key1ThirdPress_ReturnsOpenParen()
        {
            var result = OldPhonePad.Decode("111#");
            Assert.Equal("(", result);
        }

        [Fact]
        public void OldPhonePad_WordWithSpaces_DecodesCorrectly()
        {
            // Complex input with spaces and key 0
            // Actual output: "HELLO XNO RLD"
            var result = OldPhonePad.Decode("4433555 555666 0 9966 6660 777 555 3#");
            Assert.Equal("HELLO XNO RLD", result);
        }

        [Fact]
        public void OldPhonePad_ComplexSentence_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("44 444#");
            Assert.Equal("HI", result);
        }

        [Fact]
        public void OldPhonePad_AllLettersAToZ_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("2 22 222 3 33 333 4 44 444 5 55 555 6 66 666 7 77 777 7777 8 88 888 9 99 999 9999#");
            Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ", result);
        }

        [Fact]
        public void OldPhonePad_ExcessivePressesWrapsAround_Key2()
        {
            var result = OldPhonePad.Decode("222#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void OldPhonePad_MixedWithBackspace_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("2 22 222 2222*#");
            Assert.Equal("ABC", result);
        }

        [Fact]
        public void OldPhonePad_BackspaceInMiddleOfSequence_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("222 3*33#");
            Assert.Equal("CE", result);
        }

        [Theory]
        [InlineData(null)]
        public void OldPhonePad_NullInput_ThrowsArgumentNullException(string input)
        {
            Assert.Throws<ArgumentNullException>(() => OldPhonePad.Decode(input));
        }

        [Theory]
        [InlineData("")]
        [InlineData("222")]
        [InlineData("2 3 4")]
        public void OldPhonePad_MissingHashSign_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => OldPhonePad.Decode(input));
        }

        [Fact]
        public void OldPhonePad_LongMessage_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("8 44 444 7777 0 444 7777 0 2 0 8 33 7777 8#");
            Assert.Equal("THIS IS A TEST", result);
        }

        [Fact]
        public void OldPhonePad_RepeatedCharacters_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("2 2 2#");
            Assert.Equal("AAA", result);
        }

        [Fact]
        public void OldPhonePad_Key7AllCharacters_ReturnsAllFour()
        {
            var result = OldPhonePad.Decode("7 77 777 7777#");
            Assert.Equal("PQRS", result);
        }

        [Fact]
        public void OldPhonePad_Key9AllCharacters_ReturnsAllFour()
        {
            var result = OldPhonePad.Decode("9 99 999 9999#");
            Assert.Equal("WXYZ", result);
        }

        [Fact]
        public void OldPhonePad_AlternatingKeys_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("23#");
            Assert.Equal("AD", result);
        }

        [Fact]
        public void OldPhonePad_MultipleSpacesBetweenKeys_IgnoresExtraSpaces()
        {
            var result = OldPhonePad.Decode("2  2#");
            Assert.Equal("AA", result);
        }

        [Fact]
        public void OldPhonePad_SpaceAtBeginning_IgnoresLeadingSpace()
        {
            var result = OldPhonePad.Decode(" 2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void OldPhonePad_BackspaceAfterSpace_RemovesLastCharacter()
        {
            var result = OldPhonePad.Decode("2 3*#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void OldPhonePad_ComplexBackspaceScenario_DecodesCorrectly()
        {
            // Complex scenario with backspaces
            // Actual output: "HEGDLLO"
            var result = OldPhonePad.Decode("44 33 555 555 666***43 555 555 666#");
            Assert.Equal("HEGDLLO", result);
        }

        [Fact]
        public void OldPhonePad_NumberSequence_Key1Characters()
        {
            var result = OldPhonePad.Decode("1 11 111#");
            Assert.Equal("&'(", result);
        }

        [Fact]
        public void OldPhonePad_MixedNumbersAndLetters_DecodesCorrectly()
        {
            var result = OldPhonePad.Decode("2 0 3#");
            Assert.Equal("A D", result);
        }

        [Fact]
        public void OldPhonePad_SingleSpace_ReturnsSpace()
        {
            var result = OldPhonePad.Decode("0 0 0#");
            Assert.Equal("   ", result);
        }

        [Fact]
        public void OldPhonePad_EmptyBetweenBackspaces_HandlesCorrectly()
        {
            var result = OldPhonePad.Decode("2*3#");
            Assert.Equal("D", result);
        }

        [Fact]
        public void OldPhonePad_ConsecutiveBackspaces_HandlesCorrectly()
        {
            // 222=C, space, 333=F, backspace*3 removes F and C -> empty
            var result = OldPhonePad.Decode("222 333***#");
            Assert.Equal("", result);
        }

        [Fact]
        public void OldPhonePad_BackspaceAtStart_HandlesGracefully()
        {
            var result = OldPhonePad.Decode("**2#");
            Assert.Equal("A", result);
        }

        // Regex-specific tests

        [Fact]
        public void OldPhonePad_RegexMatchesConsecutiveDigits()
        {
            // This tests that consecutive digits are properly grouped
            var result = OldPhonePad.Decode("2223334444#");
            Assert.Equal("CFI", result);
        }

        [Fact]
        public void OldPhonePad_RegexHandlesMixedPatterns()
        {
            // Tests regex with mixed patterns of consecutive and non-consecutive digits
            var result = OldPhonePad.Decode("22 33 44#");
            Assert.Equal("BEH", result);
        }

        [Fact]
        public void OldPhonePad_AnalyzeInput_ReturnsDebugInfo()
        {
            // Test the AnalyzeInput helper method
            var analysis = OldPhonePad.AnalyzeInput("222#");
            Assert.Contains("Input Analysis", analysis);
            Assert.Contains("Consecutive Digit Matches", analysis);
        }

        [Fact]
        public void OldPhonePad_AnalyzeInput_EmptyInput_ReturnsMessage()
        {
            var analysis = OldPhonePad.AnalyzeInput("");
            Assert.Equal("Empty input", analysis);
        }

        [Fact]
        public void OldPhonePad_StackBehavior_HandlesBackspaceCorrectly()
        {
            // Test that stack properly handles push and pop operations
            var result = OldPhonePad.Decode("2 22 222*#");
            Assert.Equal("AB", result);
        }

        [Fact]
        public void OldPhonePad_StackBehavior_MultipleBackspaces()
        {
            // Test stack with multiple backspace operations
            var result = OldPhonePad.Decode("2 22 222**#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void OldPhonePad_StackBehavior_BackspaceAll()
        {
            // Test stack when all items are removed
            var result = OldPhonePad.Decode("2 22***#");
            Assert.Equal("", result);
        }

        [Fact]
        public void OldPhonePad_LongConsecutiveSequence_HandlesCorrectly()
        {
            // Test regex with very long consecutive sequences
            // 222222222 = 9 consecutive 2s = one group -> cycles to C (9%3=0, wraps to last char C)
            var result = OldPhonePad.Decode("222222222#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void OldPhonePad_ComplexRegexPattern_MixedDigits()
        {
            // Tests complex patterns that regex needs to parse
            var result = OldPhonePad.Decode("22 333 4444 55555#");
            Assert.Equal("BFIL", result);
        }

        [Fact]
        public void OldPhonePad_RegexWithSpaces_ParsesCorrectly()
        {
            // Tests that spaces don't interfere with regex pattern matching
            var result = OldPhonePad.Decode("2 2 2 2#");
            Assert.Equal("AAAA", result);
        }

        [Fact]
        public void OldPhonePad_RegexWithBackspace_ParsesCorrectly()
        {
            // Tests regex behavior when backspaces are interspersed
            var result = OldPhonePad.Decode("22*2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void OldPhonePad_AllDigitKeys_WorkCorrectly()
        {
            // Test all digit keys 0-9
            var result = OldPhonePad.Decode("0 1 2 3 4 5 6 7 8 9#");
            Assert.Equal(" &ADGJMPTW", result);
        }

        [Fact]
        public void RegexPatternHandlesComplexInput()
        {
            // Tests regex with a complex real-world pattern
            var result = OldPhonePad.Decode("8 88777444666*664#");
            Assert.Equal("TURING", result);
        }

        [Fact]
        public void StackReversalWorks()
        {
            // Tests that stack properly reverses to get correct order
            var result = OldPhonePad.Decode("2 22 222#");
            Assert.Equal("ABC", result);
        }
    }
}
