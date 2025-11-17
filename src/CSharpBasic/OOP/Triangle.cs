namespace CSharpBasic.OOP;

public class Triangle : Shape
{
    public override void Draw()
    {
        Console.WriteLine("Drawing Triangle");
        base.Draw();
    }
}