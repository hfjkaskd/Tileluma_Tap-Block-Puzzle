namespace BlockTap.Gameplay
{
    public readonly struct BlockTapCoordinate
    {
        public BlockTapCoordinate(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public int Column { get; }
        public int Row { get; }
    }
}
