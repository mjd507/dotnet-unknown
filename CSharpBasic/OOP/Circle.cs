namespace CSharpBasic.OOP;

public class Circle : Shape
{
    public override void Draw()
    {
        Console.WriteLine("Drawing circle");
        base.Draw();
    }
}