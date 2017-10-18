/*
 * Copyright (c) 2015-2017, Jillian England
 * License: GPL 3
 */
using System;

namespace Utility.DataGeneration
{
    public static class RandomGen
    {
        private static readonly Random Lcg = new Random();

        #region Random Number Tools (Its better to use one sequence than several, using several will overlap sooner, be less 'random', than just using one.)

        public static int Next(int minValue, int maxValue)
        {
            return Lcg.Next(minValue, maxValue);
        }

        #endregion
    }
}