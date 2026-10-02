/* ----- ----- ----- ----- */
// NumberInput.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/02
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Text;

namespace Engine.UI.Utils
{
    /// <summary>
    /// Text rules of a whole-number field (pure, no drawing; used by <c>UINumberField</c>): only
    /// the ASCII digits 0-9 are accepted (no sign, dot, full-width digits or other numerals);
    /// the text never has leading zeros (except a single "0") and never exceeds the largest
    /// value (it is replaced by it at once); a value below the smallest one - including an
    /// empty text - is only corrected when the edit is committed, because reaching "15" with a
    /// minimum of 2 passes through "1". The smallest value is at least 0.
    /// </summary>
    public static class NumberInput
    {
        /// <summary>Whether <paramref name="c"/> is an ASCII digit (0-9 only).</summary>
        public static bool IsDigit(char c) => c >= '0' && c <= '9';

        /// <summary><paramref name="text"/> (null as empty) with everything but the ASCII digits removed (what a paste is reduced to).</summary>
        public static string DigitsOnly(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
                if (IsDigit(c))
                    sb.Append(c);
            return sb.ToString();
        }

        /// <summary>Number of characters of the largest text a field with this <paramref name="max"/> takes.</summary>
        public static int MaxLength(int max) => Math.Max(0, max).ToString().Length;

        /// <summary>
        /// The text and caret to keep after an edit produced <paramref name="text"/> with the
        /// caret at <paramref name="caret"/>: non-digits dropped, leading zeros stripped (a lone
        /// "0" stays), and when the value exceeds <paramref name="max"/> the text becomes
        /// <paramref name="max"/> with the caret at its end. A value below the minimum is left alone.
        /// </summary>
        public static (string Text, int Caret) Correct(string text, int caret, int max)
        {
            text ??= string.Empty;
            caret = Math.Clamp(caret, 0, text.Length);

            // Drop non-digits, keeping the caret after the same characters.
            var digits = new StringBuilder(text.Length);
            int newCaret = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (!IsDigit(text[i]))
                    continue;
                digits.Append(text[i]);
                if (i < caret)
                    newCaret++;
            }
            string result = digits.ToString();
            caret = newCaret;

            // Leading zeros (all but a last single zero).
            int zeros = 0;
            while (zeros < result.Length - 1 && result[zeros] == '0')
                zeros++;
            if (zeros > 0)
            {
                result = result.Substring(zeros);
                caret = Math.Max(0, caret - zeros);
            }

            if (result.Length > 0 && Parse(result) > max)
            {
                result = Math.Max(0, max).ToString();
                caret = result.Length;
            }
            return (result, caret);
        }

        /// <summary>
        /// The legal value for <paramref name="text"/> when an edit ends: empty (no digits)
        /// gives <paramref name="min"/>; otherwise the value kept within
        /// <paramref name="min"/>..<paramref name="max"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="min"/> is above <paramref name="max"/>.</exception>
        public static int Commit(string text, int min, int max)
        {
            if (min > max)
                throw new ArgumentException($"min {min} is above max {max}.", nameof(min));

            string digits = DigitsOnly(text);
            if (digits.Length == 0)
                return min;
            return (int)Math.Clamp(Parse(digits), min, max);
        }

        /// <summary>The value of a digits-only text; too large for a long is <c>long.MaxValue</c>.</summary>
        private static long Parse(string digits) => long.TryParse(digits, out long value) ? value : long.MaxValue;
    }
}
