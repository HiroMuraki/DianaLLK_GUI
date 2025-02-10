public readonly struct Coordinate
{
    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Coordinate NullCoordinate = new(-1, -1);

    public int X { get; }

    public int Y { get; }

    public Coordinate Up => new(X, Y + 1);

    public Coordinate Down => new(X, Y - 1);

    public Coordinate Left => new(X - 1, Y);

    public Coordinate Right => new(X + 1, Y);

    public static float SqrDistance(Coordinate a, Coordinate b)
    {
        return ((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y));
    }

    public static bool operator ==(Coordinate left, Coordinate right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Coordinate left, Coordinate right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"[X = {X}, Y = {Y}]";
    }
}