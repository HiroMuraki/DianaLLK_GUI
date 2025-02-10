public struct Coordinate
{
    public Coordinate(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public static readonly Coordinate NullCoordinate = new(-1, -1);

    public int Row { get; set; }

    public int Column { get; set; }

    public static bool operator ==(Coordinate left, Coordinate right)
    {
        return left.Row == right.Row && left.Column == right.Column;
    }

    public static bool operator !=(Coordinate left, Coordinate right)
    {
        return !(left == right);
    }

    public readonly override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public readonly override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public readonly override string ToString()
    {
        return $"[Row = {Row}, Column = {Column}]";
    }
}