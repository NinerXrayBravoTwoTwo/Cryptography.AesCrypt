using System.Collections.Generic;

namespace Utility.DataGeneration
{
    /// <summary>
    ///     This generic class returns random subsets of an array.  It is often
    ///     used for random testing on large data sets.
    /// </summary>
    public static class RandomSelect<T>
    {
        /// <summary>
        ///     Return a random set of 'count' items from a larger set.
        ///     If count is larger than list.Length then the entire list is returned in random order.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static T[] Group(T[] list, int count)
        {
            var items = new List<T>(list);

            var size = list.Length <= count ? list.Length : count;

            var result = new List<T>();

            while (size-- > 0)
            {
                var next = RandomGen.Next(0, items.Count); // RGen.Next is a static random number call
                result.Add(items[next]);
                items.RemoveAt(next);
            }

            return result.ToArray();
        }
    }
}