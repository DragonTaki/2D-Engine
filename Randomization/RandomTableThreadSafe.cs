/* ----- ----- ----- ----- */
// RandomTable.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Engine.Randomization
{
    /// <summary>
    /// Thread-safe version of RandomTable.
    /// Uses locking to ensure concurrent safety when accessed by multiple threads.
    /// </summary>
    public class RandomTableThreadSafe : IRandomProvider
    {
        private readonly int[] _intTable;
        private readonly float[] _floatTable;
        private readonly double[] _doubleTable;
        private int _index;
        private readonly int _tableSize;
        private readonly object _lockObj = new object();

        /// <summary>
        /// Initializes a new instance of the RandomTableThreadSafe class.
        /// </summary>
        /// <param name="size">The number of pre-generated random entries.</param>
        /// <param name="seed">The seed value to ensure deterministic results.</param>
        public RandomTableThreadSafe(int size, int seed)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Table size must be positive.");

            _tableSize = size;
            _intTable = new int[size];
            _floatTable = new float[size];
            _doubleTable = new double[size];
            Random rand = new Random(seed);

            for (int i = 0; i < size; i++)
            {
                _intTable[i] = rand.Next();
                // Narrowing a double just below 1.0 to float can round up to exactly 1.0f,
                // breaking the documented [0, 1) range; clamp to the largest float below 1.
                float f = (float)rand.NextDouble();
                _floatTable[i] = f < 1f ? f : MathF.BitDecrement(1f);
                _doubleTable[i] = rand.NextDouble();
            }

            _index = 0;
        }

        /// <summary>
        /// Gets the next pre-generated integer in the table.
        /// </summary>
        /// <returns>An integer from the table.</returns>
        public int NextInt()
        {
            lock (_lockObj)
            {
                int value = _intTable[_index];
                Advance();
                return value;
            }
        }

        /// <summary>
        /// Gets the next pre-generated integer in the range [0, max).
        /// </summary>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>An integer in [0, max).</returns>
        public int NextInt(int max)
        {
            lock (_lockObj)
            {
                // Same contract as System.Random.Next(int): max < 0 throws, max == 0 gives 0
                // (instead of a DivideByZeroException / negative results).
                if (max < 0)
                    throw new ArgumentOutOfRangeException(nameof(max), max, "max must be non-negative.");
                if (max == 0)
                    return 0;
                return NextInt() % max;
            }
        }

        /// <summary>
        /// Gets the next pre-generated integer in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>An integer in [min, max).</returns>
        public int NextInt(int min, int max)
        {
            lock (_lockObj)
            {
                // Same contract as System.Random.Next(int, int): min > max throws, min == max gives min.
                // long arithmetic so a wide range (e.g. negative min) can't overflow max - min.
                if (min > max)
                    throw new ArgumentOutOfRangeException(nameof(min), min, "min must not exceed max.");
                if (min == max)
                    return min;
                return (int)(min + NextInt() % ((long)max - min));
            }
        }

        /// <summary>
        /// Gets the next pre-generated float in the range [0.0, 1.0).
        /// </summary>
        /// <returns>A float in [0.0, 1.0).</returns>
        public float NextFloat()
        {
            lock (_lockObj)
            {
                float value = _floatTable[_index];
                Advance();
                return value;
            }
        }

        /// <summary>
        /// Gets the next pre-generated float in the range [0.0, max).
        /// </summary>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A float in [0.0, max).</returns>
        public float NextFloat(float max)
        {
            // The product can round up to exactly max (e.g. the largest float below 1 times a
            // large max); keep the documented exclusive bound.
            float value = NextFloat() * max;
            return max > 0f && value >= max ? MathF.BitDecrement(max) : value;
        }

        /// <summary>
        /// Gets the next pre-generated float in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A float in [min, max).</returns>
        public float NextFloat(float min, float max)
        {
            // Computed in one step (not min + NextFloat(max - min)): the sum can round up to
            // exactly max even when the scaled part stays below max - min.
            float value = min + NextFloat() * (max - min);
            return max > min && value >= max ? MathF.BitDecrement(max) : value;
        }

        /// <summary>
        /// Gets the next pre-generated double in the range [0.0, 1.0).
        /// </summary>
        /// <returns>A double in [0.0, 1.0).</returns>
        public double NextDouble()
        {
            lock (_lockObj)
            {
                double value = _doubleTable[_index];
                Advance();
                return value;
            }
        }

        /// <summary>
        /// Gets the next pre-generated double in the range [0.0, max).
        /// </summary>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A double in [0.0, max).</returns>
        public double NextDouble(double max)
        {
            // Same exclusive-bound guard as NextFloat(float).
            double value = NextDouble() * max;
            return max > 0d && value >= max ? Math.BitDecrement(max) : value;
        }

        /// <summary>
        /// Gets the next pre-generated double in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A double in [min, max).</returns>
        public double NextDouble(double min, double max)
        {
            // Same exclusive-bound guard as NextFloat(float, float).
            double value = min + NextDouble() * (max - min);
            return max > min && value >= max ? Math.BitDecrement(max) : value;
        }

        /// <summary>
        /// Advances the current _index (circular). Not locked itself: every caller already holds <c>_lockObj</c>.
        /// </summary>
        private void Advance()
        {
            _index = (_index + 1) % _tableSize;
        }

        /// <summary>
        /// Resets the _index back to 0 in a thread-safe way.
        /// </summary>
        public void Reset()
        {
            lock (_lockObj)
            {
                _index = 0;
            }
        }
    }
}
