namespace CSharpBasic.OOP;

public class Rectangle : Shape
{
    public override void Draw()
    {
        Console.WriteLine("Drawing Rectangle");
        base.Draw();
    }
}