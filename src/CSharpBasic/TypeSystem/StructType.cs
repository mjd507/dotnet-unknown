namespace CSharpBasic.TypeSystem;

public class StructType
{
    public struct Coords(int x, int y)
    {
        public int X { get; set; } = x;

        public int Y { get; set; } = y;
    }
}