namespace CSharpBasic.TypeSystem;

public class StructType
{
    public readonly struct Coords(int x, int y)
    {
        public int X { get; init; } = x;

        public int Y { get; init; } = y;
    }

}
