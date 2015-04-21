namespace npuzzle
{
    public class SuccessorList
    {
        private SuccessorList _Next;

        public SuccessorList Next
        {
            get { return _Next ?? (_Next = new SuccessorList(_stateSize)); }
        }
        public int Size { get; private set; }
        private readonly int _stateSize;
        private readonly byte[][] items;

        public SuccessorList(int stateSize)
        {
            Size = 0;
            _stateSize = stateSize;
            items = new byte[4][];
            for (int i = 0; i < 4; i++)
            {
                items[i] = new byte[_stateSize];
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
