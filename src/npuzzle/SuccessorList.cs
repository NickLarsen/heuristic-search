namespace npuzzle
{
    public class SuccessorList
    {
        public SuccessorList Next { get; set; }
        public int Size { get; private set; }
        private readonly byte[][] items;

        public SuccessorList(int stateSize)
        {
            Size = 0;
            items = new byte[4][];
            for (int i = 0; i < 4; i++)
            {
                items[i] = new byte[stateSize];
            }
        }

        public byte[] Push()
        {
            return items[Size++]; // post, so you get the item then the incrment happens
        }

        public byte[] Pop()
        {
            return items[--Size]; // pre, so Size changes then you get the item
        }
    }
}
