namespace npuzzle
{
    static class ByteArrayExtensions
    {
        public static void Swap(this byte[] array, int a, int b)
        {
            byte temp = array[a];
            array[a] = array[b];
            array[b] = temp;
        }
    }
}
