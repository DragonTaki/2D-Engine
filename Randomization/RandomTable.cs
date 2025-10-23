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
    /// High-performance reusable random number generator using pre-generated value tables
    /// for predictable and efficient pseudo-random access.
    /// </summary>
    public class RandomTable : IRandomProvider
    {
        private readonly int[] _intTable;
        private readonly float[] _floatTable;
        private readonly double[] _doubleTable;
        private int _index;
        private readonly int _tableSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomTable"/> class.
        /// </summary>
        /// <param name="size">The number of pre-generated values in each table.</param>
        /// <param name="seed">The seed used to generate deterministic random sequences.</param>
        public RandomTable(int size, int seed)
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
            int value = _intTable[_index];
            Advance();
            return value;
        }

        /// <summary>
        /// Gets the next pre-generated integer in the range [0, max).
        /// </summary>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>An integer in [0, max).</returns>
        public int NextInt(int max)
        {
            return NextInt() % max;
        }

        /// <summary>
        /// Gets the next pre-generated integer in the range [min, max).
        /// </summary>
        /// <param name="min">Inclusive lower bound.</param>
        /// <param name="max">Exclusive upper bound.</param>
        /// <returns>An integer in [min, max).</returns>
        public int NextInt(int min, int max)
        {
            return min + NextInt(max - min);
        }

        /// <summary>
        /// Gets the next pre-generated float in the range [0.0, 1.0).
        /// </summary>
        /// <returns>A float in [0.0, 1.0).</returns>
        public float NextFloat()
        {
            float value = _floatTable[_index];
            Advance();
            return value;
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
            double value = _doubleTable[_index];
            Advance();
            return value;
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
        /// Advances the current _index to the next element in the tables (circular buffer).
        /// </summary>
        private void Advance()
        {
            _index = (_index + 1) % _tableSize;
        }

        /// <summary>
        /// Resets the _index to the beginning of the table.
        /// </summary>
        public void Reset() => _index = 0;
    }
}
