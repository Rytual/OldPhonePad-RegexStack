using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OldPhonePad.RegexStack
{
    /// <summary>
    /// Decodes old-style mobile phone keypad input using regex preprocessing and a Stack data structure.
    /// Uses pattern matching to identify key sequences and a stack for backspace operations.
    /// </summary>
    public static class OldPhonePad
    {
        // Key mappings for the phone keypad
        private static readonly Dictionary<char, string> keyMap = new Dictionary<char, string>
        {
            { '0', " " },
            { '1', "&'(" },
            { '2', "ABC" },
            { '3', "DEF" },
            { '4', "GHI" },
            { '5', "JKL" },
            { '6', "MNO" },
            { '7', "PQRS" },
            { '8', "TUV" },
            { '9', "WXYZ" }
        };

        // Regex pattern to match consecutive digit sequences
        // Matches one or more consecutive occurrences of the same digit (0-9)
        private static readonly Regex consecutiveDigitsPattern = new Regex(@"(\d)\1*", RegexOptions.Compiled);

        // Regex pattern to validate input format
        // Ensures input contains only valid characters: digits, spaces, asterisks, and ends with #
        private static readonly Regex validInputPattern = new Regex(@"^[0-9\s\*]*#$", RegexOptions.Compiled);

        /// <summary>
        /// Decodes input from an old mobile phone keypad into text.
        /// Uses regex to preprocess the input and a Stack to handle backspace operations.
        /// </summary>
        /// <param name="input">
        /// The input string representing key presses. Should contain:
        /// - Digits 0-9 (representing key presses)
        /// - Spaces (pauses between different keys)
        /// - Asterisks (*) for backspace
        /// - Must end with # to send the message
        /// </param>
        /// <returns>The decoded text string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
        /// <exception cref="ArgumentException">Thrown when input format is invalid.</exception>
        /// <example>
        /// <code>
        /// string result = OldPhonePad.OldPhonePad("33#"); // Returns "E"
        /// string result = OldPhonePad.OldPhonePad("227*#"); // Returns "B"
        /// string result = OldPhonePad.OldPhonePad("4433555 555666#"); // Returns "HELLO"
        /// </code>
        /// </example>
        public static string OldPhonePad(string input)
        {
            // Validate input
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrEmpty(input) || input[input.Length - 1] != '#')
            {
                throw new ArgumentException("Input must end with '#' to send the message.", nameof(input));
            }

            if (!validInputPattern.IsMatch(input))
            {
                throw new ArgumentException("Input contains invalid characters.", nameof(input));
            }

            // Remove the trailing '#' for processing
            string sequence = input.Substring(0, input.Length - 1);

            // Use a stack to build the result, which makes backspace operations elegant
            var resultStack = new Stack<char>();

            // Process the input character by character
            int i = 0;
            while (i < sequence.Length)
            {
                char currentChar = sequence[i];

                // Handle backspace - pop from stack
                if (currentChar == '*')
                {
                    if (resultStack.Count > 0)
                    {
                        resultStack.Pop();
                    }
                    i++;
                    continue;
                }

                // Handle pause (space between different keys)
                if (currentChar == ' ')
                {
                    i++;
                    continue;
                }

                // Handle digit keys using regex to find consecutive presses
                if (char.IsDigit(currentChar))
                {
                    // Use regex to match consecutive occurrences of the same digit
                    Match match = consecutiveDigitsPattern.Match(sequence, i);

                    if (match.Success && match.Index == i)
                    {
                        char key = match.Groups[1].Value[0];
                        int pressCount = match.Length;

                        // Get the character for this key and press count
                        char? character = GetCharacterForKey(key, pressCount);

                        if (character.HasValue)
                        {
                            resultStack.Push(character.Value);
                        }

                        // Move index forward by the length of the match
                        i += match.Length;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    // Skip any other characters
                    i++;
                }
            }

            // Convert stack to string (need to reverse since stack is LIFO)
            return StackToString(resultStack);
        }

        /// <summary>
        /// Gets the character corresponding to a key and the number of times it was pressed.
        /// </summary>
        /// <param name="key">The numeric key (0-9).</param>
        /// <param name="pressCount">The number of consecutive presses.</param>
        /// <returns>The corresponding character, or null if invalid.</returns>
        private static char? GetCharacterForKey(char key, int pressCount)
        {
            if (!keyMap.ContainsKey(key))
            {
                return null;
            }

            string characters = keyMap[key];

            // Handle wrapping: if press count exceeds available characters, take modulo
            // But if it's exactly divisible, use the last character
            if (pressCount < 1)
            {
                return null;
            }

            // Clamp to valid range
            if (pressCount > characters.Length)
            {
                pressCount = characters.Length;
            }

            // Convert to 0-based index
            return characters[pressCount - 1];
        }

        /// <summary>
        /// Converts a Stack of characters to a string in the correct order.
        /// </summary>
        /// <param name="stack">The stack containing characters.</param>
        /// <returns>The string representation of the stack contents.</returns>
        private static string StackToString(Stack<char> stack)
        {
            // Stack is LIFO, so we need to reverse it
            var chars = new char[stack.Count];
            int index = stack.Count - 1;

            foreach (char c in stack)
            {
                chars[index--] = c;
            }

            return new string(chars);
        }

        /// <summary>
        /// Analyzes the input string and returns debugging information about the regex matches.
        /// Useful for understanding how regex preprocessing works.
        /// </summary>
        /// <param name="input">The input string to analyze.</param>
        /// <returns>A string containing debug information.</returns>
        public static string AnalyzeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "Empty input";
            }

            var result = new StringBuilder();
            result.AppendLine("Input Analysis:");
            result.AppendLine($"Original: {input}");
            result.AppendLine();

            // Remove trailing #
            string sequence = input.EndsWith("#") ? input.Substring(0, input.Length - 1) : input;

            result.AppendLine("Consecutive Digit Matches:");
            MatchCollection matches = consecutiveDigitsPattern.Matches(sequence);

            foreach (Match match in matches)
            {
                if (match.Length > 0 && char.IsDigit(match.Value[0]))
                {
                    char key = match.Value[0];
                    int count = match.Length;
                    char? character = GetCharacterForKey(key, count);

                    result.AppendLine($"  Position {match.Index}: '{match.Value}' -> Key '{key}' pressed {count} time(s) -> '{character?.ToString() ?? "null"}'");
                }
            }

            return result.ToString();
        }
    }
}
