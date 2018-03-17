namespace ChromeTabs
{
    public class TabReorder
    {
        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public TabReorder(int from, int to)
        {
            FromIndex = from;
            ToIndex = to;
        }
    }
}
