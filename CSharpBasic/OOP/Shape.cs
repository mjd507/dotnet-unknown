namespace CSharpBasic.OOP;

public class Shape
{
    public int X { get; init; }
    public int Y { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public virtual void Draw()
    {
        Console.WriteLine("Performing base class drawing tasks");
    }
}