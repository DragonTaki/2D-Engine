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
            _tableSize = size;
            _intTable = new int[size];
            _floatTable = new float[size];
            _doubleTable = new double[size];
            Random rand = new Random(seed);

            for (int i = 0; i < size; i++)
            {
                _intTable[i] = rand.Next();
                _floatTable[i] = (float)rand.NextDouble();
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
                return min + NextInt(max - min);
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
            return NextFloat() * max;
        }

        /// <summary>
        /// Gets the next pre-generated float in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A float in [min, max).</returns>
        public float NextFloat(float min, float max)
        {
            return min + NextFloat(max - min);
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
            return NextDouble() * max;
        }

        /// <summary>
        /// Gets the next pre-generated double in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>A double in [min, max).</returns>
        public double NextDouble(double min, double max)
        {
            return min + NextDouble(max - min);
        }

        /// <summary>
        /// Thread-safe advancement of the current _index.
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
