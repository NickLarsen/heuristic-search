using System;
using System.Collections;

namespace npuzzle
{
    static class Extensions
    {
        public static void Swap<T>(this T[] array, int a, int b)
        {
            T temp = array[a];
            array[a] = array[b];
            array[b] = temp;
        }
    }
}
