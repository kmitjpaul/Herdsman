using System;

namespace Helpers
{
    public static class Number
    {
        public static (T a, T b) Swap<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) < 0 ? (a, b) : (b, a);
        }
    }
}