/* ----- ----- ----- ----- */
// MathUtil.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/09
// Update Date: 2025/05/09
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;

namespace Engine.Mathematics
{
    public static class MathUtil
    {
        /// <summary>
        /// Sigmoids a float to [0.0, 1.0] range.
        /// </summary>
        public static float Sigmoid01(float value)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-value));
        }

        /// <summary>
        /// Clamps a float value to a valid byte range (0 ~ 255).
        /// </summary>
        public static int ClampToByte(float value)
        {
            return Math.Max(0, Math.Min(255, (int)value));
        }

        /// <summary>
        /// Clamps a float value to a closed to zero but non zero value.
        /// </summary>
        public static float ClampMinFloat(float value, float min = 0.0001f)
        {
            return Math.Max(min, value);
        }

        public static float ClampF(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float LinearMap(float inputValue, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            if ((maxInput - minInput) == 0) return float.NaN;
            // Normalize inputValue to the range [0, 1]
            float normalizedInput = (inputValue - minInput) / (maxInput - minInput);

            // Map normalized values ​​to the range [minOutput, maxOutput]
            return minOutput + normalizedInput * (maxOutput - minOutput);
        }

        public static float SigmoidMap(float inputValue, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            if ((maxInput - minInput) == 0) return float.NaN;
            // Normalize inputValue to the range [0, 1]
            float normalizedInput = (inputValue - minInput) / (maxInput - minInput);

            // Sigmoid function, mapping values ​​from [0, 1] to the [0, 1] interval
            float sigmoidValue = 1 / (1 + (float)Math.Exp(-10 * (normalizedInput - 0.5f))); // 10 is the smoothness factor of the adjustment

            // Map the sigmoid value to the range [minOutput, maxOutput]
            return minOutput + sigmoidValue * (maxOutput - minOutput);
        }
        public static float LogMap(float inputValue, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            if ((maxInput - minInput) == 0) return float.NaN;
            // Normalize inputValue to the range [0, 1]
            float normalizedInput = (inputValue - minInput) / (maxInput - minInput);

            // Using logarithmic to map
            float logMappedValue = (float)Math.Log10(1 + 9 * normalizedInput); // Use 9 to control the magnitude of the logarithm

            // Map the log value to the range [minOutput, maxOutput]
            return minOutput + logMappedValue * (maxOutput - minOutput);
        }

        public static float NormalDistributionMap(float inputValue, float minInput, float maxInput, float minOutput, float maxOutput)
        {
            if ((maxInput - minInput) == 0) return float.NaN;
            // Normalize inputValue to the range [0, 1]
            float normalizedInput = (inputValue - minInput) / (maxInput - minInput);

            // Normal distribution function, with a mean of 0.5 and a standard deviation of 0.1
            float gaussianValue = (float)(1 / Math.Sqrt(2 * Math.PI * 0.1f) * Math.Exp(-Math.Pow(normalizedInput - 0.5f, 2) / (2 * 0.1f * 0.1f)));

            // Map the normal distribution value to the range [minOutput, maxOutput]
            return minOutput + gaussianValue * (maxOutput - minOutput);
        }

        /// <summary>
        /// Performs linear interpolation between two float values.
        /// </summary>
        public static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }
        public static Vector2F Lerp(Vector2F from, Vector2F to, float t)
        {
            float x = from.X + (to.X - from.X) * t;
            float y = from.Y + (to.Y - from.Y) * t;
            return new Vector2F(x, y);
        }

        public static Color LerpColor(Color from, Color to, float t)
        {
            int r = ClampToByte(from.R + (int)((to.R - from.R) * t));
            int g = ClampToByte(from.G + (int)((to.G - from.G) * t));
            int b = ClampToByte(from.B + (int)((to.B - from.B) * t));
            return Color.FromArgb(r, g, b);
        }
    }
}
