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

    public readonly int X => Column;

    public readonly int Y => Row;

    public readonly Coordinate Up => new(Y + 1, X);

    public readonly Coordinate Down => new(Y - 1, X);

    public readonly Coordinate Left => new(Y, X - 1);

    public readonly Coordinate Right => new(Y, X + 1);

    public static float SqrDistance(Coordinate a, Coordinate b)
    {
        return ((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y));
    }

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